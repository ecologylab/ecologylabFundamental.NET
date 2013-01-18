using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using Ecologylab.Collections;
using Simpl.Fundamental.Generic;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Messages;
using Simpl.OODSS.PlatformSpecifics;
using Simpl.Serialization;

namespace Simpl.OODSS.Distributed.Client
{
    public class WebSocketOODSSClient
    {
        #region Data Member

        private static readonly object SyncLock = new object();

        private readonly BlockingCollection<RequestQueueObject> _requestQueue;
        private readonly ConcurrentDictionary<long, RequestQueueObject> _pendingRequests;
        private readonly BlockingCollection<ResponseQueueObject> _responseQueue;

        public string ServerAddress { get; private set; }
        public int PortNumber { get; private set; }

        public Scope<object> ObjectRegistry { get; set; }

        protected int ReconnectAttemps = ClientConstants.ReconnectAttempts;
        protected int WaitBetweenReconnectAttemps = ClientConstants.WaitBetweenReconnectAttempts;
        protected ReconnectedBlocker Blocker;

        private long _uidIndex = 1;
        

        public SimplTypesScope TranslationScope { get; private set; }

        private string _sessionId;

        private CancellationTokenSource _cancellationTokenSource;

        private volatile bool _isRunning = false;

        private static ManualResetEventSlim SendDone = new ManualResetEventSlim(false);
        //private static ManualResetEventSlim ReceiveDone = new ManualResetEventSlim(false);

        private static string _response = String.Empty;

        #endregion Data Member

        #region WebSocketComponent

        // in windowsRT, use Windows.Networking.Sockets.StreamWebSocket
        // in .NET, use System.Net.WebSockets.ClientWebSocket
        private object _webSocketClient;

        //TODO:  a large enough buffer, should be more careful about how to use buffer
        private byte[] _readBuffer = new byte[40000];   // 40000 is a emperial number that works well with streamsocket
        private Uri _serverUri;
        const string WebSocketPrefix = "ws://";

        private readonly object syncLock = new object();

        // background working thread

        #endregion WebSocketComponent

        #region Constructor

        /// <summary>
        /// Initialze a websocket OODSS client object
        /// </summary>
        /// <param name="ipAddress">server's ip address</param>
        /// <param name="portNumber">server's port number</param>
        /// <param name="translationScope">TranslationScope for OODSS messages</param>
        /// <param name="objectRegistry">application object scope</param>
        public WebSocketOODSSClient(String ipAddress, int portNumber, SimplTypesScope translationScope,
                                    Scope<object> objectRegistry)
        {
            ObjectRegistry = objectRegistry;
            TranslationScope = translationScope;
            ObjectRegistry.Add(SessionObjects.SessionId, _sessionId);
            ServerAddress = ipAddress;
            PortNumber = portNumber;


            _pendingRequests = new ConcurrentDictionary<long, RequestQueueObject>();
            _requestQueue = new BlockingCollection<RequestQueueObject>(new ConcurrentQueue<RequestQueueObject>());
            _responseQueue = new BlockingCollection<ResponseQueueObject>(new ConcurrentQueue<ResponseQueueObject>());


        }

        #endregion Constructor

        /// <summary>
        /// Starting the client, setting up the websocketclient
        /// </summary>
        public async Task<bool> StartAsync()
        {
            _isRunning = true;

            _cancellationTokenSource = new CancellationTokenSource();
            //_cancellationTokenSource.Token.Register(PerformDisconnect);
            
            String uri = WebSocketPrefix + ServerAddress + ":" + PortNumber;
            // create and connect
            _webSocketClient = OODSSPlatformSpecifics.Get().CreateWebSocketClientObject();
            // connect to the server
            await OODSSPlatformSpecifics.Get().ConnectWebSocketClientAsync(_webSocketClient, new Uri(uri),
                                                                     _cancellationTokenSource.Token);

            OODSSPlatformSpecifics.Get().CreateWorkingThreadAndStart(SendMessageWorker, ReceiveMessageWorker, WebSocketDataReceiver, _cancellationTokenSource.Token);

            return await ConnectOODSSServerAsync();
        }

        public async Task StopClient()
        {
            lock(syncLock)
            {
                _isRunning = false;
            }
            PerformDisconnect();
            _cancellationTokenSource.Cancel();
            
        }

        private void PerformDisconnect()
        {
            Debug.WriteLine("Performing Disconnect");
            OODSSPlatformSpecifics.Get().DisconnectWebSocketClientAsync(_webSocketClient);
        }

        private void UnableToRestorePreviousConnection(string sessionId, string newId)
        {
            // do something;
        }

        /// <summary>
        /// establishing the oodss protocol by sending initConnectionRequest
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectOODSSServerAsync()
        {
            if (ConnectedImpl())
            {
                // get initResponse to see if it is correct; 
                ResponseMessage initResponse = await RequestAsync(new InitConnectionRequest(_sessionId));
                var initConnectionResponse = initResponse as InitConnectionResponse;
                if (initConnectionResponse != null)
                {
                    Debug.WriteLine("Received initial connection response");
                    if (_sessionId == null)
                    {
                        // get a sesssion id
                        _sessionId = initConnectionResponse.SessionId;
                        Debug.WriteLine("SessionId: " + _sessionId);
                    }
                    else if (_sessionId == initConnectionResponse.SessionId)
                    {
                        // received the same session id, do nothing;
                    }
                    else
                    {
                        // update the sessionId
                        string newId = initConnectionResponse.SessionId;
                        UnableToRestorePreviousConnection(_sessionId, newId);
                        _sessionId = newId;
                    }
                }
            }
            return Connected();
        }

        private bool ConnectedImpl()
        {
            return Connected() || CreateConnection();
        }

        private bool CreateConnection()
        {
            if (!Connected())
            {
                OODSSPlatformSpecifics.Get().ConnectWebSocketClientAsync(_webSocketClient, _serverUri, CancellationToken.None);
            }
            return Connected();
        }
        
        private bool Connected()
        {
            return OODSSPlatformSpecifics.Get().WebSocketIsConnected(_webSocketClient);
        }

        /// <summary>
        /// Send Request Message asynchronously and get response
        /// </summary>
        /// <param name="request">request message</param>
        /// <returns>response message</returns>
        public async Task<ResponseMessage> RequestAsync(RequestMessage request)
        {
            TaskCompletionSource<ResponseMessage> tcs = new TaskCompletionSource<ResponseMessage>();
            long currentMessageUid = GenerateUid();

            RequestQueueObject queueRequest = new RequestQueueObject(request, currentMessageUid, tcs);

            _pendingRequests.Put(currentMessageUid, queueRequest);
            _requestQueue.Add(queueRequest);

            return await tcs.Task;
        }


        /// <summary>
        /// Generate a uid for the request message
        /// </summary>
        /// <returns></returns>
        public long GenerateUid()
        {
            lock (SyncLock)
                return _uidIndex++;
        }

        private void ProcessUpdate(UpdateMessage updateMessage)
        {
            updateMessage.ProcessUpdate(ObjectRegistry);
        }

        private void ProcessResponse(ResponseMessage responseMessage)
        {
            responseMessage.ProcessResponse(ObjectRegistry);
        }

        /// <summary>
        /// process the incoming message. if it is a response message, add it to unprocessedResponse, 
        /// if it is an unpdate message, process it immediately
        /// </summary>
        /// <param name="incomingMessage">incoming message in serialized form</param>
        /// <param name="incomingUid">incoming message's uid</param>
        private void ProcessString(string incomingMessage, long incomingUid)
        {
            ServiceMessage message = TranslateStringToServiceMessage(incomingMessage);
            if (message == null)
            {
                Debug.WriteLine("Deserialized failed");
            }
            else
            {
                if (message is ResponseMessage)
                {
                    Debug.WriteLine("Got a response message");

                    // add the _reponse to the queue of response. 
                    _responseQueue.Add(new ResponseQueueObject((ResponseMessage)message, incomingUid));
                }
                else if (message is UpdateMessage)
                {
                    // if it is an update message, process it immediately 
                    Debug.WriteLine("Got an updateMessage");
                    ProcessUpdate((UpdateMessage)message);
                }
            }
            //return _response;
        }

        /// <summary>
        /// deserialize the incoming message to service message. 
        /// </summary>
        /// <param name="incomingMessage">incoming message in serialized form</param>
        /// <returns>deserialized service message</returns>
        private ServiceMessage TranslateStringToServiceMessage(string incomingMessage)
        {
            ServiceMessage responseMessage = (ServiceMessage)TranslationScope.Deserialize(incomingMessage, StringFormat.Xml);
            if (responseMessage == null)
                return null;

            return responseMessage;
        }

        /// <summary>
        /// Generate String from requestMessage
        /// </summary>
        /// <param name="request">request message</param>
        /// <returns>request message in serialized form</returns>
        private string GenerateStringFromRequest(RequestMessage request)
        {
            StringBuilder requestStringBuilder = new StringBuilder();
            SimplTypesScope.Serialize(request, requestStringBuilder, StringFormat.Xml);
            return requestStringBuilder.ToString();
        }

        /// <summary>
        /// prepare the message and send it to the websocket server
        /// </summary>
        /// <param name="requestObject"></param>
        private async Task CreatePacketFromMessageAndSend(RequestQueueObject requestObject)
        {
            long uid = requestObject.Uid;
            string requestString = GenerateStringFromRequest(requestObject.RequestMessage);
            byte[] uidBytes = BitConverter.GetBytes(uid);
            byte[] messageBytes = Encoding.UTF8.GetBytes(requestString);
            byte[] outMessage = new byte[uidBytes.Length + messageBytes.Length];
            Buffer.BlockCopy(uidBytes, 0, outMessage, 0, uidBytes.Length);
            Buffer.BlockCopy(messageBytes, 0, outMessage, uidBytes.Length, messageBytes.Length);
            await OODSSPlatformSpecifics.Get().SendMessageFromWebSocketClientAsync(_webSocketClient, outMessage);
        }

        #region Websocket send and receive

        /// <summary>
        /// background worker getting request from the blocking queue and send it out.
        /// </summary>
        private async void SendMessageWorker()
        {
            Debug.WriteLine("Entering OODSS Send Message Loop");
            while (_isRunning && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    //Hold the thread here until it receives a request to be processed.
                    RequestQueueObject q = _requestQueue.Take(_cancellationTokenSource.Token);

                    // push pull
                    Debug.WriteLine("Trying to send the string: {0}", q.Uid);
                    
                    //SendDone = new ManualResetEventSlim(false);
                    await CreatePacketFromMessageAndSend(q);
                    //SendDone.Wait(_cancellationTokenSource.Token);

                    Debug.WriteLine("---sent Request: {0}", q.Uid);
                }
                catch(OperationCanceledException e)
                {
                    Debug.WriteLine("SendWorker: The operation was cancelled." + e.CancellationToken);
                    break;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Caught Exception :\n " + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// background worker getting response from the blocking queue and process it.
        /// </summary>
        private void ReceiveMessageWorker()
        {
            Debug.WriteLine("Entering OODSS Receive Message Loop");
            while (_isRunning && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    ResponseQueueObject responseQueueObject = _responseQueue.Take(_cancellationTokenSource.Token);
                    ProcessResponse(responseQueueObject.ResponseMessage);

                    RequestQueueObject requestQueueObject;
                    _pendingRequests.TryGetValue(responseQueueObject.Uid, out requestQueueObject);
                    if (requestQueueObject == null)
                    {
                        Debug.WriteLine("No pending request with Uid: {0}", responseQueueObject.Uid);
                    }
                    else
                    {
                        TaskCompletionSource<ResponseMessage> taskCompletionSource = requestQueueObject.Tcs;
                        if (taskCompletionSource != null)
                        {
                            Debug.WriteLine("---Finished Request: {0}", requestQueueObject.Uid);
                            taskCompletionSource.TrySetResult(responseQueueObject.ResponseMessage);
                        }
                    }
                }
                catch(OperationCanceledException e)
                {
                    Debug.WriteLine("Receiving worker: The operation was cancelled." + e.CancellationToken);
                    break;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Caught Exception :\n " + e.StackTrace);
                }
            }
        }

        private async void WebSocketDataReceiver()
        {
             while (_isRunning && !_cancellationTokenSource.Token.IsCancellationRequested)
             {
                 try
                 {
                     // websocket client receive data from stream
                     byte[] incomingData = await
                                           OODSSPlatformSpecifics.Get()
                                                                 .ReceiveMessageFromWebSocketClientAsync(
                                                                     _webSocketClient, _readBuffer,
                                                                     _cancellationTokenSource.Token);
                     // process the byte data
                     long uid = BitConverter.ToInt64(incomingData, 0);
                     string message = Encoding.UTF8.GetString(incomingData, 8, incomingData.Length - 8);
                     Debug.WriteLine("Got the message: " + message + " uid: " + uid);

                     ProcessString(message, uid);
                 }
                 catch (OperationCanceledException e)
                 {
                     Debug.WriteLine("DataReceiver: The operation was cancelled." + e.CancellationToken);
                     break;
                 }
                 catch (Exception e)
                 {
                     Debug.WriteLine("DataReceiver: The operation was cancelled.");
                     break;
                 }
             }
        }

        #endregion
    }
}
