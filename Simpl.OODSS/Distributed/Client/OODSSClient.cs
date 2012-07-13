using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simpl.Fundamental.Generic;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using ecologylab.collections;
using ecologylab.serialization;

/* 
 * 
 *  This is the initial OODSS for C#.  It uses the async library.
 *  Use the connect function to connect to an OODSS client and the GetResponse to
 *  get and send data.
 */

namespace Simpl.OODSS.Distributed.Client
{
    public class OODSSClient 
    {
        private Socket _clientSocket;

        private readonly Thread _sendMesssageThread;
        private readonly Thread _receiveMesssageThread;
        
        private readonly BlockingCollection<QueueObject> _requestQueue;

        private readonly ConcurrentDictionary<int, QueueObject> _pendingRequests;

        static int _uid = 1;

        private CancellationTokenSource _cancellationTokenSource;

        private static readonly SimplTypesScope ServicesTranslationScope = DefaultServicesTranslations.Get();

        private bool _isRunning = true;

        private static ManualResetEventSlim SendDone = new ManualResetEventSlim(false);
        private static ManualResetEventSlim ReceiveDone = new ManualResetEventSlim(false);

        private static string _response = String.Empty;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public SimplTypesScope SimplTypesScope { get; set; }
        public Scope<object> ObjectRegistry { get; set; }

        public OODSSClient(String host, int port, SimplTypesScope simplTypesScope, Scope<object> objectRegistry)
        {
            _sendMesssageThread         = new Thread(SendMessageWorker);
            _receiveMesssageThread      = new Thread(ReceiveMessageWorker);
            _pendingRequests            = new ConcurrentDictionary<int, QueueObject>();
            _requestQueue               = new BlockingCollection<QueueObject>(new ConcurrentQueue<QueueObject>());

            Host                        = host;
            Port                        = port;
            ObjectRegistry              = objectRegistry;

            //Add the OODSS messages to the scope.
            simplTypesScope.AddTranslations(ServicesTranslationScope);

            SimplTypesScope             = simplTypesScope;
            
            var initState = new InitConnectionRequest();
            int uid = _uid;
            QueueObject q = new QueueObject(initState, uid, null);
            _requestQueue.Add(q);
        }

        private void AddRequest(RequestMessage obj)
        {
            QueueObject q = new QueueObject( obj, _uid, null);
            _requestQueue.Add(q);
        }

        public void Start() 
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.Register(PerformDisconnect);

            try 
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Host);//"achilles.cse.tamu.edu");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);
                _clientSocket = new Socket(AddressFamily.InterNetwork,  SocketType.Stream, ProtocolType.Tcp);
                _clientSocket.Connect(Host, Port);

                Console.WriteLine("Client socket with host ({0}) connected? {1}.", Host, _clientSocket.Connected);

                _sendMesssageThread.Start();
                _receiveMesssageThread.Start();
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Incomplete. 
        /// TODO: Send and confirm that disconnect has occured
        /// </summary>
        private void PerformDisconnect()
        {
            Console.WriteLine("Performing Disconnect");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"> </param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestAsync(RequestMessage requestMessage)
        {
            TaskCompletionSource<ResponseMessage> tcs = new TaskCompletionSource<ResponseMessage>();
            int uid = _uid++;

            QueueObject queueRequest = new QueueObject(requestMessage, uid, tcs);

            _pendingRequests.Put(uid, queueRequest);
            _requestQueue.Add(queueRequest);
            
            return await tcs.Task;
        }

        private string MessageForXml(ElementState requestObj, int uidUsed)
        {
            string msg = SimplTypesScope.Serialize(requestObj, StringFormat.Xml);
            string ret = String.Format("content-length:{0}\r\nuid:{1}\r\n\r\n{2}", msg.Length, uidUsed, msg);
            return ret;
        }

        private class QueueObject
        {
            public RequestMessage RequestMessage { get; private set; }
            public int Uid { get; private set; }
            public TaskCompletionSource<ResponseMessage> Tcs { get; private set; }

            public QueueObject(RequestMessage requestMessage, int uid, TaskCompletionSource<ResponseMessage> tcs )
            {
                RequestMessage = requestMessage;
                Uid = uid;
                Tcs = tcs;
            }
        }


        private class ReceiveStateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024*1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        private static void Receive(Socket client)
        {
            ReceiveDone = new ManualResetEventSlim(false);
            try
            {
                // Create the state object.
                ReceiveStateObject state = new ReceiveStateObject {workSocket = client};

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, ReceiveStateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                ReceiveStateObject state = (ReceiveStateObject)ar.AsyncState;
                Socket client = state.workSocket;
                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    string value = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    
                    state.sb.Append(value);
                    //  Get the rest of the data.
                    Console.WriteLine("Got so far:\n"+value);

                    ProcessMessageString(value);
//                    else
//                    {
//                        Console.WriteLine("Getting More data. ");
//                        client.BeginReceive(state.buffer, 0, ReceiveStateObject.BufferSize, 0, ReceiveCallback, state);    
//                    }
                }
                else
                {
//                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        _response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    ReceiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ProcessMessageString(string value)
        {
            int msgLengthIndex = value.IndexOf(":") + 1;
            string intString = value.Substring(msgLengthIndex, value.IndexOf("\r\n", msgLengthIndex) - msgLengthIndex);
            int msgLength = int.Parse(intString);
            if (msgLength > 0)
            {
                int expectedLength = value.IndexOf("<") + msgLength;
                _response = value.Substring(0, expectedLength);
                ReceiveDone.Set();
                if (value.Length > expectedLength)
                {
                    string sb = value.Substring(expectedLength);
                    ProcessMessageString(sb);
                }
            }
        }

        private static void Send(Socket client, String data) {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.
                Socket client = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                SendDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendMessageWorker()
        {
            Console.WriteLine("Entering OODSS Send Message loop");
            while (_isRunning)
            {
                try
                {
                    //Console.WriteLine("Top of loop" );
                    //Hold the thread here until it recieves a request to be processed.
                    QueueObject q = _requestQueue.Take(_cancellationTokenSource.Token);

                    //Push pull
                    Console.WriteLine("Trying to send the string:" + q.Uid);

                    SendDone = new ManualResetEventSlim(false);
                    string messageForXml = MessageForXml(q.RequestMessage, q.Uid);
                    Send(_clientSocket, messageForXml);
                    SendDone.Wait(_cancellationTokenSource.Token);
                    Console.WriteLine("--- Sent Request : {0}", q.Uid);
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
            Console.WriteLine("Entering OODSS Recieve Message loop");
            while (_isRunning)
            {
                try
                {
                    Receive(_clientSocket);
                    ReceiveDone.Wait(_cancellationTokenSource.Token);

                    int uidIndex = _response.IndexOf("uid:") + 4;
                    string intString = _response.Substring(uidIndex, _response.IndexOf("\r\n", uidIndex) - uidIndex);
                    int responseUid = int.Parse(intString);

                    ServiceMessage serviceMessage = (ServiceMessage)SimplTypesScope.Deserialize(_response.Substring(_response.IndexOf('<')), StringFormat.Xml);
                    if (serviceMessage == null)
                    {
                        throw new ArgumentNullException(string.Format("Received a null {0} object. Deserialization failed?", "serviceMessage"));
                    }

                    if (serviceMessage is UpdateMessage)
                    {
                        var updateMessage = serviceMessage as UpdateMessage;
                        updateMessage.ProcessUpdate(ObjectRegistry);
                    }
                    else if (serviceMessage is ResponseMessage)
                    {
                        var responseMessage = serviceMessage as ResponseMessage;
                        responseMessage.ProcessResponse(ObjectRegistry);

                        QueueObject queueObject;
                        _pendingRequests.TryGetValue(responseUid, out queueObject);
                        if (queueObject == null)
                        {
                            Console.WriteLine("No pending request with Uid: {0}", responseUid);
                        }
                        else
                        {
                            TaskCompletionSource<ResponseMessage> taskCompletionSource = queueObject.Tcs;
                            if (taskCompletionSource != null)
                            {
                                Console.WriteLine("--- Finished Request : {0}", queueObject.Uid);
                                taskCompletionSource.TrySetResult(responseMessage);
                            }
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
    }
}
