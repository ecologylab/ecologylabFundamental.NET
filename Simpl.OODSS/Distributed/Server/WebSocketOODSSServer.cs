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
using ecologylab.collections;


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
        public WebSocketOODSSServer(SimplTypesScope requestTranslationScope, Scope<object> applicationObjectScope,
			int idleConnectionTimeout=-1, int maxMessageSize=-1)
            :base(0, Dns.GetHostAddresses(Dns.GetHostName()), requestTranslationScope, applicationObjectScope, 
            idleConnectionTimeout, maxMessageSize)
        {
            MaxMessageSize = maxMessageSize + NetworkConstants.MaxHttpHeaderLength;
            TranslationScope = requestTranslationScope;

            ApplicationObjectScope = applicationObjectScope;

            ApplicationObjectScope.Add(SessionObjects.SessionsMap, ClientSessionManagerMap);
            ApplicationObjectScope.Add(SessionObjects.WebSocketOODSSServer, this);

            _serverInstance = this;

            SetUpWebSocketServer();
            StartServer();
        }
        #endregion Constructor

        #region WebSocketServer
        private void SetUpWebSocketServer()
        {
            WebSocketServer = new WebSocketServer();
            WebSocketServer.NewDataReceived += WebSocketServer_NewDataReceived;
            WebSocketServer.Setup(new RootConfig(), new ServerConfig
            {
                Port = 2018,
                Ip = "Any",
                MaxConnectionNumber = 100,
                MaxCommandLength = 100000,
                Mode = SocketMode.Async,
                Name = "SuperWebSocket Server"
            }, SocketServerFactory.Instance);
        }

        private void WebSocketServer_NewDataReceived(WebSocketSession session, byte[] e)
        {
            ProcessRead(session, e);
        }

        protected void StartServer()
        {
            WebSocketServer.Start();
        }

        public void StopServer()
        {
            WebSocketServer.Stop();
        }

        #endregion WebSocketServer


        #region Member Methods

        /// <summary>
        /// parse the 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="e"></param>
        public void ProcessRead(WebSocketSession session, byte[] e)
        {
            if (e.Length > 0)
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

                // process the message
                CurrentData = e;
                //obtain Uid.
                long uid = BitConverter.ToInt64(CurrentData, 0);

                //obtain message.
                int messageBytesLength = CurrentData.Length - 8;
                byte[] messageBytes = new byte[messageBytesLength];
                Buffer.BlockCopy(CurrentData, 8, messageBytes, 0, messageBytesLength);
                CurrentMessage = Encoding.UTF8.GetString(messageBytes);
                Console.WriteLine("Got the message: " + CurrentMessage);

                ResponseMessage responseMessage = cm.ProcessString(CurrentMessage, uid);

                // send responseMessage back.
                CreatePacketFromMessageAndSend(uid, responseMessage, session);            
            }
        }

        /// <summary>
        /// Generate WebSocketClientSessionManager
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="translationScope"></param>
        /// <param name="globalScope"></param>
        /// <returns></returns>
        protected override BaseSessionManager GenerateContextManager(
            string sessionId, SimplTypesScope translationScope, Scope<object> globalScope)
        {
            return new WebSocketClientSessionManager(sessionId, translationScope, globalScope, this);
        }

        /// <summary>
        /// helper function generate and send byte array message to client session. 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="message"></param>
        /// <param name="session"></param>
        private void CreatePacketFromMessageAndSend(long uid, ServiceMessage message, WebSocketSession session)
        {
            StringBuilder responseMessageStringBuilder = new StringBuilder();
            SimplTypesScope.Serialize(message, responseMessageStringBuilder, StringFormat.Xml);
            string req = responseMessageStringBuilder.ToString();        
            Console.WriteLine("send message: "+ req + " uid "+uid);

            byte[] uidBytes = BitConverter.GetBytes(uid);
            byte[] messageBytes = Encoding.UTF8.GetBytes(req);
            byte[] outMessage = new byte[uidBytes.Length + messageBytes.Length];
            Buffer.BlockCopy(uidBytes, 0, outMessage, 0, uidBytes.Length);
            Buffer.BlockCopy(messageBytes, 0, outMessage, uidBytes.Length, messageBytes.Length);
            session.SendResponse(outMessage);
        }

        /// <summary>
        /// called by the session manager to send out update message. 
        /// </summary>
        /// <param name="receivingSessionId"></param>
        /// <param name="update"></param>
        internal void SendUpdateMessage(string receivingSessionId, UpdateMessage update)
        {
            WebSocketClientSessionManager sessionManager;
            if(ClientSessionManagerMap.TryGetValue(receivingSessionId, out sessionManager))
            {            
                CreatePacketFromMessageAndSend(-1, update, sessionManager.Session);
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
        /// restore old sessionManager for recovered session.
        /// </summary>
        /// <param name="incomingSessionId"></param>
        /// <param name="newSessionManager"></param>
        /// <returns></returns>
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
