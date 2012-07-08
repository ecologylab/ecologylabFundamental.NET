using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simpl.Fundamental.Generic;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Distributed.Impl;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using WebSocket4Net;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Client
{
    public class WebSocketOODSSClient
    {
        #region Data Member

        private readonly Thread _sendMessageThread;
        private readonly Thread _receiveMessageThread;

        private readonly BlockingCollection<RequestQueueObject> _requestQueue;
        private readonly ConcurrentDictionary<long, RequestQueueObject> _pendingRequests;
        private readonly BlockingCollection<ResponseQueueObject> _responseQueue; 

        public string ServerAddress { get; private set; }
        public int PortNumber { get; private set; }

        public Scope<object> ObjectRegistry { get; set; }

        //protected readonly Dictionary<long, RequestMessage> UnfulfilledRequests = new Dictionary<long, RequestMessage>();

        //protected readonly ConcurrentDictionary<long, ResponseMessage> UnprocessedResponse = new ConcurrentDictionary<long, ResponseMessage>(); 

        protected int ReconnectAttemps = ClientConstants.ReconnectAttempts;
        protected int WaitBetweenReconnectAttemps = ClientConstants.WaitBetweenReconnectAttempts;
        protected ReconnectedBlocker Blocker;

        private long _uidIndex = 1;

        public SimplTypesScope TranslationScope { get; private set; }

        private string _sessionId;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _isRunning = true;

        private static ManualResetEventSlim SendDone = new ManualResetEventSlim(false);
        private static ManualResetEventSlim ReceiveDone = new ManualResetEventSlim(false);

        private static string _response = String.Empty;

        #endregion

        #region WebSocketComponent

        private AutoResetEvent _messageReceiveEvent = new AutoResetEvent(false);
        private AutoResetEvent _dataReceiveEvent = new AutoResetEvent(false);
        private AutoResetEvent _openedEvent = new AutoResetEvent(false);
        private AutoResetEvent _closeEevnt = new AutoResetEvent(false);

        private WebSocket _webSocketClient;

        protected string CurrentMessage { get; private set; }
        protected byte[] CurrentData { get; private set; }

        #endregion WebSocketComponent

        #region Constructor

        public WebSocketOODSSClient(String ipAddress, int portNumber, SimplTypesScope translationScope, Scope<object> objectRegistry,
            int maxMessageLengthChars=NetworkConstants.DefaultMaxMessageLengthChars, WebSocketVersion version = WebSocketVersion.Rfc6455) 
        {
            ObjectRegistry              = objectRegistry;
            TranslationScope            = translationScope;
            ObjectRegistry.Add(SessionObjects.SessionId, _sessionId);
            ServerAddress               = ipAddress;
            PortNumber                  = portNumber;

            _sendMessageThread          = new Thread(SendMessageWorker);
            _receiveMessageThread       = new Thread(ReceiveMessageWorker);
            _pendingRequests            = new ConcurrentDictionary<long, RequestQueueObject>();
            _requestQueue               = new BlockingCollection<RequestQueueObject>(new ConcurrentQueue<RequestQueueObject>());
            _responseQueue              = new BlockingCollection<ResponseQueueObject>(new ConcurrentQueue<ResponseQueueObject>());
        }

        #endregion Constructor

        #region Connection Related

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.Register(PerformDisconnect);

            try
            {
                string webSocketPrefix = "ws://";
                String uri = webSocketPrefix + ServerAddress + ":" + PortNumber + "/websocket";

                //_webSocketClient = new WebSocket(uri, "basic", version);
                _webSocketClient = new WebSocket(uri);
                _webSocketClient.Opened += WebSocketClientOpened;
                _webSocketClient.Closed += WebSocketClientClosed;
                _webSocketClient.DataReceived += WebSocketClientDataReceived;
                _webSocketClient.MessageReceived += WebSocketClientMessageReceived;
                _sendMessageThread.Start();
                _receiveMessageThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void StopClient()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
        }

        public void PerformDisconnect()
        {
            Console.WriteLine("Performing Disconnect");
        }

        /// <summary>
        /// make connection to the websocket server. when connection is made, send an initConnectionRequest
        /// to get a sessionId. 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectAsync()
        {
            if (ConnectedImpl())
            {
                // get initResponse to see if it is correct; 
                ResponseMessage initResponse = await RequestAsync(new InitConnectionRequest(_sessionId));

                if (initResponse is InitConnectionResponse)
                {
                    Console.WriteLine("Received initial connection response");
                    if (_sessionId == null)
                    {
                        // get a sesssion id
                        _sessionId = ((InitConnectionResponse) initResponse).SessionId;
                        Console.WriteLine("SessionId: " + _sessionId);
                    }
                    else if (_sessionId == ((InitConnectionResponse) initResponse).SessionId)
                    {
                        // received the same session id, do nothing;
                    }
                    else
                    {
                        // update the sessionId
                        string newId = ((InitConnectionResponse) initResponse).SessionId;
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
                _webSocketClient.Open();
                if (!_openedEvent.WaitOne(2000))
                Console.WriteLine("Handshake failed");
            }
            return Connected();
        }

        private bool Connected()
        {
            return _webSocketClient.Handshaked;
        }

        public void Disconnect()
        {
            _webSocketClient.Close();
        }

        ///// <summary>
        ///// reconnect to the server, restore the queue. how to restore in websocket
        ///// </summary>
        //protected void Reconnect()
        //{
        //    Console.WriteLine("attempting to reconnect...");
        //    int reconnectsRemaining = ReconnectAttemps;
        //    if (reconnectsRemaining < 0)
        //    {
        //        reconnectsRemaining = 1;
        //    }

        //    while (!Connected() && reconnectsRemaining > 0)
        //    {
        //        NullOut();
        //        if (!ConnectAsync() && --reconnectsRemaining < 0)
        //        {
        //            //wait some time
        //        }
        //    }

        //    if (Connected())
        //    {
        //        // put unfulfilled requests back to the queue
        //        lock (UnfulfilledRequests)
        //        {
        //            List<PreppedRequest> rerequests = new List<PreppedRequest>(UnfulfilledRequests.Values);
        //            rerequests.Sort();
        //            foreach (PreppedRequest req in rerequests)
        //            {
        //                EnqueueRequestForSending(req);
        //            }
        //        }

        //    }
        //    else
        //    {
        //        Stop();
        //    }
        //}

//        protected void HandleDisconnectingMessages()
//        {
//            Console.WriteLine("sending disconnect request");
//            SendMessageAsync(DisconnectRequest.ReusableInstance);
//        }
//
        private void UnableToRestorePreviousConnection(string sessionId, string newId)
        {
            // do nothing;
        }
//
//        protected void NullOut()
//        {
//            // do something to clear the socket.
//        }
//
//        protected bool ShutdownOK()
//        {
//            return !(UnfulfilledRequests.Count > 0);
//        }

        #endregion Connection Related

        #region OODSS message related

        private class RequestQueueObject
        {
            public RequestMessage RequestMessage { get; private set; }
            public long Uid { get; private set; }
            public TaskCompletionSource<ResponseMessage> Tcs { get; private set; }

            public RequestQueueObject(RequestMessage requestMessage, long uid, TaskCompletionSource<ResponseMessage> tcs)
            {
                RequestMessage = requestMessage;
                Uid = uid;
                Tcs = tcs;
            }
        }

        private class ResponseQueueObject
        {
            public ResponseMessage ResponseMessage { get; private set; }
            public long Uid { get; private set; }
            public ResponseQueueObject(ResponseMessage responseMessage, long uid)
            {
                ResponseMessage = responseMessage;
                Uid = uid;
            }
        }

        public async Task<ResponseMessage> RequestAsync(RequestMessage request)
        {
            TaskCompletionSource<ResponseMessage> tcs = new TaskCompletionSource<ResponseMessage>();
            long currentMessageUid = GenerateUid();

            RequestQueueObject queueRequest = new RequestQueueObject(request, currentMessageUid, tcs);

            _pendingRequests.Put(currentMessageUid, queueRequest);
            _requestQueue.Add(queueRequest);

            return await tcs.Task;
        }

//        /// <summary>
//        /// send a requestMessage, wait for the response, and process the response. 
//        /// </summary>
//        /// <param name="request"></param>
//        /// <param name="timeOutMillis"></param>
//        /// <returns>response message</returns>
//        public async Task<ResponseMessage> SendMessageAsync (RequestMessage request)
//        {
//            //Prepare the request, add to the unfulfilledRequests;
//            string requestString = GenerateStringFromRequest(request);
//            long currentMessageUid = GenerateUid();
//
//            AddToUnfulfilledRequests(currentMessageUid, request);
//
//            CreatePacketFromMessageAndSend(currentMessageUid, requestString);
//
//            // get response
//            ResponseMessage responseMessage = await GetResponseMessageAsync(currentMessageUid);
//            ProcessResponse(responseMessage);
//            
//            // remove the response and request from the dictionary. 
//            RemoveFromUnprocessedResponse(currentMessageUid);
//            RemoveFromUnfulfilledRequests(currentMessageUid); 
//
//            return responseMessage;
//        }
//

//
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long GenerateUid()
        {
            return _uidIndex++;
        }
//
//        [MethodImpl(MethodImplOptions.Synchronized)]
//        private void AddToUnfulfilledRequests(long uid, RequestMessage request)
//        {
//            UnfulfilledRequests.Add(uid, request);
//        }
//
//        [MethodImpl(MethodImplOptions.Synchronized)]
//        private void RemoveFromUnprocessedResponse(long uid)
//        {
//            UnprocessedResponse.Remove(uid);
//        }
//
//        [MethodImpl(MethodImplOptions.Synchronized)]
//        private void RemoveFromUnfulfilledRequests(long uid)
//        {
//            UnfulfilledRequests.Remove(uid);
//        }
//
//        /// <summary>
//        /// query the collection of unprocessed responses. obtain the one with the correct uid.
//        /// </summary>
//        /// <param name="uid"></param>
//        /// <returns>response message</returns>
//        private async Task<ResponseMessage> GetResponseMessageAsync(long uid)
//        {
//            ResponseMessage message;
//            while (!UnprocessedResponse.ContainsKey(uid))
//            {
//                //await Task.Delay(50);
//                await Task.Factory.StartNew(WaitAWhile);
//            }
//            // contains the key
//            lock (UnprocessedResponse)
//            {
//                UnprocessedResponse.TryGetValue(uid, out message);
//                
//            }
//            return message;
//        }
//
//        private void WaitAWhile()
//        {
//            Thread.Sleep(50);
//        }

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
        /// <param name="incomingMessage"></param>
        /// <param name="incomingUid"></param>
        private void ProcessString(string incomingMessage, long incomingUid)
        {
            ServiceMessage message = TranslateStringToServiceMessage(incomingMessage);
            if (message == null)
            {
                Console.WriteLine("Deserialized failed");
            }
            else
            {
                if (message is ResponseMessage)
                {
                    Console.WriteLine("Got a response message");
                    
                    // add the _reponse to the queue of response. 
                    _responseQueue.Add(new ResponseQueueObject((ResponseMessage)message, incomingUid));
                }
                else if (message is UpdateMessage)
                {
                    // if it is an update message, process it immediately 
                    Console.WriteLine("Got an updateMessage");
                    ProcessUpdate((UpdateMessage) message);
                }
            }
            //return _response;
        }

        /// <summary>
        /// deserialize the incoming message to service message. 
        /// </summary>
        /// <param name="incomingMessage"></param>
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
        /// <param name="request"></param>
        /// <returns></returns>
        private string GenerateStringFromRequest(RequestMessage request)
        {
            StringBuilder requestStringBuilder = new StringBuilder();
            SimplTypesScope.Serialize(request, requestStringBuilder, StringFormat.Xml);
            return requestStringBuilder.ToString();
        }

        /// <summary>
        /// prepare the message and send it out through websocket
        /// </summary>
        /// <param name="requestObject"></param>
        private void CreatePacketFromMessageAndSend(RequestQueueObject requestObject)
        {
            long uid = requestObject.Uid;
            string requestString = GenerateStringFromRequest(requestObject.RequestMessage);
            byte[] uidBytes = BitConverter.GetBytes(uid);
            byte[] messageBytes = Encoding.UTF8.GetBytes(requestString);
            byte[] outMessage = new byte[uidBytes.Length+messageBytes.Length];
            Buffer.BlockCopy(uidBytes, 0, outMessage, 0, uidBytes.Length);
            Buffer.BlockCopy(messageBytes, 0, outMessage, uidBytes.Length, messageBytes.Length);
            _webSocketClient.Send(outMessage, 0, outMessage.Length);
            SendDone.Set();
        }

        private void SendMessageWorker()
        {
            Console.WriteLine("Entering OODSS Send Message Loop");
            while(_isRunning)
            {
                try
                {
                    //Hold the thread here until it receives a request to be processed.
                    RequestQueueObject q = _requestQueue.Take(_cancellationTokenSource.Token);

                    //Push pull
                    Console.WriteLine("Trying to send the string: " + q.Uid);

                    SendDone = new ManualResetEventSlim(false);
                    CreatePacketFromMessageAndSend(q);
                    SendDone.Wait(_cancellationTokenSource.Token);
                    Console.WriteLine("---sent Request: {0}", q.Uid);        
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("The operation was cancelled." + e.TargetSite);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Caught Exception :\n " + e.StackTrace);
                }

            }
            Console.WriteLine("Performing disconnect");
        }

        private void ReceiveMessageWorker()
        {
            Console.WriteLine("Entering OODSS Receive Message loop");
            while(_isRunning)
            {
                try
                {
                    ResponseQueueObject responseQueueObject = _responseQueue.Take(_cancellationTokenSource.Token);
             
                    ProcessResponse(responseQueueObject.ResponseMessage);

                    RequestQueueObject requestQueueObject;
                    _pendingRequests.TryGetValue(responseQueueObject.Uid, out requestQueueObject);
                    if (requestQueueObject == null)
                    {
                        Console.WriteLine("No pending request with Uid: {0}", responseQueueObject.Uid);
                    }
                    else
                    {
                        TaskCompletionSource<ResponseMessage> taskCompletionSource = requestQueueObject.Tcs;
                        if (taskCompletionSource != null)
                        {
                            Console.WriteLine("--- Finished Request : {0}", requestQueueObject.Uid);
                            taskCompletionSource.TrySetResult(responseQueueObject.ResponseMessage);
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("The operation was cancelled." + e.TargetSite);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught Exception :\n " + e.StackTrace);
                }
            }
        }

        #endregion OODSS message related


        #region WebSocket Handler
        
        /// <summary>
        /// When websocketclient receive a message. process the message.
        /// the first 64bit (long) is the uid.
        /// the rest is the message.
        /// after getting the uid and message, put a MessageWithMetadata object in the queue. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocketClientDataReceived(object sender, DataReceivedEventArgs e)
        {
            //if (!_firstMessageSent)
            //{
            //    _firstMessageSent = true;
            //    return;
            //}

            CurrentData = e.Data;
            //obtain Uid.
            long uid = BitConverter.ToInt64(CurrentData, 0);

            //obtain message.
            int messageBytesLength = CurrentData.Length - 8;
            byte[] messageBytes = new byte[messageBytesLength];
            Buffer.BlockCopy(CurrentData, 8, messageBytes, 0, messageBytesLength);
            CurrentMessage = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine("Got the message: " + CurrentMessage + " uid: "+ uid);

            ProcessString(CurrentMessage, uid);

            _dataReceiveEvent.Set();
        }

        void WebSocketClientMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = e.Message;
            byte[] receivedData = Encoding.UTF8.GetBytes(message);
            CurrentData = receivedData;
            //obtain Uid.
            long uid = BitConverter.ToInt64(CurrentData, 0);

            //obtain message.
            int messageBytesLength = CurrentData.Length - 8;
            byte[] messageBytes = new byte[messageBytesLength];
            Buffer.BlockCopy(CurrentData, 8, messageBytes, 0, messageBytesLength);
            CurrentMessage = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine("Got the message: " + CurrentMessage + " uid: " + uid);

            ProcessString(CurrentMessage, uid);

            _messageReceiveEvent.Set();
        }

        void WebSocketClientClosed(object sender, EventArgs e)
        {
            _closeEevnt.Set();
        }

        void WebSocketClientOpened(object sender, EventArgs e)
        {
            _openedEvent.Set();
        }

        #endregion WebSocket Handler

    }

}
