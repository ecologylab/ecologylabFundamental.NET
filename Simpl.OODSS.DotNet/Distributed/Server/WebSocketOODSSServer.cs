using System;
using System.Net;
using System.Text;
using System.Threading;
using Simpl.Fundamental.Generic;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Distributed.Impl;
using Simpl.OODSS.Distributed.Server.ClientSessionManager;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using SuperWebSocket;
using Ecologylab.Collections;


namespace Simpl.OODSS.Distributed.Server
{
    public class WebSocketOODSSServer : AbstractServer, ServerProcessor
    {
        #region Data Members

        protected WebSocketServer WebSocketServer { get; set; }
        protected AutoResetEvent MessageReceiveEvent = new AutoResetEvent(false);
        protected AutoResetEvent DataReceiveEvent = new AutoResetEvent(false);
        protected AutoResetEvent OpenedEvent = new AutoResetEvent(false);
        protected AutoResetEvent CloseEvent = new AutoResetEvent(false);
        protected string CurrentMessage { get; private set; }
        protected byte[] CurrentData { get; private set; }

        private DictionaryList<object, WebSocketClientSessionManager>
            ClientSessionManagerMap = new DictionaryList<object, WebSocketClientSessionManager>();

        private static readonly Encoding EncodedCharSet = NetworkConstants.Charset;

        private static Decoder CharSetDecoder = EncodedCharSet.GetDecoder();

        protected int MaxMessageSize;

        private static WebSocketOODSSServer _serverInstance;

        public static WebSocketOODSSServer ServerInstance
        {
            get
            {
                if (_serverInstance == null)
                    Console.WriteLine("The instance you called is null");
                return _serverInstance;
            }
        }

        protected Thread ServerThread;

        #endregion Data Members

        #region Constructor
        /// <summary>
        /// Initialize a websocket oodss server object
        /// </summary>
        /// <param name="serverTranslationScope">translationscope for the oodss messages</param>
        /// <param name="applicationObjectScope">server object scope</param>
        /// <param name="idleConnectionTimeout"></param>
        /// <param name="maxMessageSize"></param>
        public WebSocketOODSSServer(SimplTypesScope serverTranslationScope, Scope<object> applicationObjectScope,
			int idleConnectionTimeout=-1, int maxMessageSize=-1, int port=0)
            : base(port, Dns.GetHostAddresses(Dns.GetHostName()), serverTranslationScope, applicationObjectScope, 
            idleConnectionTimeout, maxMessageSize)
        {
            MaxMessageSize = maxMessageSize + NetworkConstants.MaxHttpHeaderLength;
            TranslationScope = serverTranslationScope;

            ApplicationObjectScope = applicationObjectScope;

            ApplicationObjectScope.Add(SessionObjects.SessionsMap, ClientSessionManagerMap);
            ApplicationObjectScope.Add(SessionObjects.WebSocketOODSSServer, this);

            _serverInstance = this;

            SetUpWebSocketServer(port);
        }
        #endregion Constructor

        #region WebSocketServer
        private void SetUpWebSocketServer(int port)
        {
            WebSocketServer = new WebSocketServer();
            WebSocketServer.NewDataReceived += WebSocketServer_NewDataReceived;
            WebSocketServer.NewMessageReceived += WebSocketServer_NewMessageReceived;
            WebSocketServer.NewSessionConnected += WebSocketServer_NewSessionConnected;
            WebSocketServer.SessionClosed += WebSocketServer_SessionClosed;

            WebSocketServer.Setup(new RootConfig(), new ServerConfig
            {
                Port = port,
                Ip = "Any",
                MaxConnectionNumber = 100,
                MaxCommandLength = 100000,
                Mode = SocketMode.Async,
                Name = "SuperWebSocket Server"
            }, SocketServerFactory.Instance);
        }

        private void WebSocketServer_NewDataReceived(WebSocketSession session, byte[] e)
        {
            Console.WriteLine("new data received");
            // process the message
            CurrentData = e;
            //obtain Uid.
            Console.WriteLine("this computer is little endian: {0}", BitConverter.IsLittleEndian);
            long uid = BitConverter.ToInt64(CurrentData, 0);

            //obtain message.
            int messageBytesLength = CurrentData.Length - 8;
            byte[] messageBytes = new byte[messageBytesLength];
            Buffer.BlockCopy(CurrentData, 8, messageBytes, 0, messageBytesLength);
            CurrentMessage = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine("Got the message: " + CurrentMessage);
            ProcessRead(session, uid, CurrentMessage);
        }

        private void WebSocketServer_NewMessageReceived(WebSocketSession session, string e)
        {
            Console.WriteLine("new message received");
            byte[] messageBytes = Encoding.UTF8.GetBytes(e);
            WebSocketServer_NewDataReceived(session, messageBytes);
        }

        private void WebSocketServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("Session connected: " + session.SocketSession.RemoteEndPoint);
        }

        private void WebSocketServer_SessionClosed(WebSocketSession session, CloseReason reason)
        {
            Console.WriteLine("Session "+ session.SocketSession.RemoteEndPoint +" closed because: " + reason);
        }

        /// <summary>
        /// start server
        /// </summary>
        public override bool Start()
        {
            return WebSocketServer.Start();
        }

        /// <summary>
        ///  stop server
        /// </summary>
        public override void Stop()
        {
            WebSocketServer.Stop();
        }

        #endregion WebSocketServer


        #region Member Methods

        /// <summary>
        /// process the recieved message and uid. 
        /// if sessionManager does not exist, create add it to the client session manager map
        /// process the message and send the response message back to the client
        /// </summary>
        /// <param name="session">the client's websocket session</param>
        /// <param name="uid">uid of the message</param>
        /// <param name="message">message in serialized form</param>
        public void ProcessRead(WebSocketSession session, long uid, string message)
        {
            if (message.Length > 0)
            {
                // check and add session to the clientSessionManagerMap.
                WebSocketClientSessionManager cm;
                lock (ClientSessionManagerMap)
                {
                    if (!ClientSessionManagerMap.TryGetValue(session.SocketSession.SessionID, out cm))
                    {
                        Console.WriteLine("server creating context manager for " + session.SocketSession.RemoteEndPoint);
                        
                        string sessionId = session.SocketSession.SessionID;
                        Console.WriteLine("sessionId: "+sessionId);
                        cm = (WebSocketClientSessionManager)GenerateContextManager(sessionId, TranslationScope, ApplicationObjectScope);
                        cm.Session = session;
                        
                        ClientSessionManagerMap.Put(sessionId, cm);
                    }  
                }



                ResponseMessage responseMessage = cm.ProcessString(message, uid);

                // send responseMessage back.
                CreatePacketFromMessageAndSend(uid, responseMessage, session);            
            }
        }

        /// <summary>
        /// Generate WebSocketClientSessionManager
        /// </summary>
        /// <param name="sessionId">client's session id</param>
        /// <param name="translationScope">translation scope for the server</param>
        /// <param name="applicationObjectScope">server's application scope</param>
        /// <returns></returns>
        protected override BaseSessionManager GenerateContextManager(
            string sessionId, SimplTypesScope translationScope, Scope<object> applicationObjectScope)
        {
            return new WebSocketClientSessionManager(sessionId, translationScope, applicationObjectScope, this);
        }

        /// <summary>
        /// helper function to generate and send byte array message to client session. 
        /// </summary>
        /// <param name="uid">uid of the received message</param>
        /// <param name="message">oodss message</param>
        /// <param name="session">client's websocket session</param>
        /// 
        /// out message format
        ///  0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 .......n     
        /// |length |      uid        |        message ....|
        /// 
        private void CreatePacketFromMessageAndSend(long uid, ServiceMessage message, WebSocketSession session)
        {
            StringBuilder responseMessageStringBuilder = new StringBuilder();
            SimplTypesScope.Serialize(message, responseMessageStringBuilder, StringFormat.Xml);
            string req = responseMessageStringBuilder.ToString();        
            Console.WriteLine("send message: "+ req + " uid "+uid);

            byte[] uidBytes = BitConverter.GetBytes(uid);
            byte[] messageBytes = Encoding.UTF8.GetBytes(req);
            byte[] lengthBytes = BitConverter.GetBytes(uidBytes.Length + messageBytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);
            byte[] outMessage = new byte[lengthBytes.Length + uidBytes.Length + messageBytes.Length];

            Buffer.BlockCopy(lengthBytes, 0, outMessage, 0, lengthBytes.Length);
            Buffer.BlockCopy(uidBytes, 0, outMessage, lengthBytes.Length, uidBytes.Length);
            Buffer.BlockCopy(messageBytes, 0, outMessage, lengthBytes.Length + uidBytes.Length, messageBytes.Length);
            
            session.SendResponse(outMessage);
            //session.SendResponse(Encoding.UTF8.GetString(outMessage));
        }

        /// <summary>
        /// called by the session manager to send out update message. 
        /// </summary>
        /// <param name="sessionId">client's session id</param>
        /// <param name="updateMessage">update message</param>
        protected internal void SendUpdateMessage(string sessionId, UpdateMessage updateMessage)
        {
            WebSocketClientSessionManager sessionManager;
            if (ClientSessionManagerMap.TryGetValue(sessionId, out sessionManager))
            {
                CreatePacketFromMessageAndSend(0, updateMessage, sessionManager.Session);
            }
        }

        //public bool Invalidate(string sessionId, bool forcePermanent)
        //{
        //    WebSocketClientSessionManager cm = ClientSessionManagerMap[sessionId];

        //    bool permanent = (forcePermanent ? true : (cm == null ? true : cm.IsInvalidating()));

        //    if (permanent)
        //    {
        //        lock(ClientSessionManagerMap)
        //        {
        //            ClientSessionManagerMap.Remove(sessionId);
        //            //ClientSessionHandleMap.Remove(sessionId);
        //        }
        //    }

        //    if (cm != null)
        //    {
        //        cm.Shutdown();
        //    }

        //    return forcePermanent;
        //}

        #endregion Member Methods

        /// <summary>
        /// restore old sessionManager for recovered session. returns true if the session is restored, false if the old session doesn't exist thus cannot be restored.
        /// </summary>
        /// <param name="incomingSessionId">received sesion id information</param>
        /// <param name="newSessionManager">a new session manager</param>
        /// <returns>whether the session can be restored</returns>
        public bool RestoreContextManagerFromSessionId(string incomingSessionId, BaseSessionManager newSessionManager)
        {
            WebSocketClientSessionManager oldSessionManager;
            lock(ClientSessionManagerMap)
            {
                ClientSessionManagerMap.TryGetValue(incomingSessionId, out oldSessionManager);
            }
            if (oldSessionManager == null)
            {
                return false;
            }
            oldSessionManager.Session = ((WebSocketClientSessionManager) newSessionManager).Session;
            lock(ClientSessionManagerMap)
            {
                ClientSessionManagerMap.Remove(newSessionManager.SessionId);
            }
            return true;
        }

    }
}
