using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.Fundamental.Generic;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Distributed.Impl;
using Simpl.OODSS.Distributed.Server.ClientSessionManager;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using SuperWebSocket;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server
{
    public class WebSocketOODSSServer : AbstractServer, IServerMessages 
    {
        #region Data Members

        public WebSocketServer WebSocketServer;

        private DictionaryList<object, WebSocketClientSessionManager>
            ClientSessionManagerMap = new DictionaryList<object, WebSocketClientSessionManager>();

        private Dictionary<string, WebSocketClientSessionManager> _sessionForSessionIdMap;

        private DictionaryList<object, SessionHandle> ClientSessionHandleMap = new DictionaryList<object, SessionHandle>();

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

        #endregion Data Members

        #region Constructor
        public WebSocketOODSSServer(SimplTypesScope requestTranslationScope, Scope<object> applicationObjectScope,
			int idleConnectionTimeout, int maxMessageSize)
            :base(0, Dns.GetHostAddresses(Dns.GetHostName()), requestTranslationScope, applicationObjectScope, 
            idleConnectionTimeout, maxMessageSize)
        {
            MaxMessageSize = maxMessageSize + NetworkConstants.MaxHttpHeaderLength;
            TranslationScope = requestTranslationScope;

            applicationObjectScope.Add(SessionObjects.SessionsMap, ClientSessionHandleMap);
            applicationObjectScope.Add(SessionObjects.WebSocketOODSSServer, this);

            InstantiateBufferPools(MaxMessageSize);

            _sessionForSessionIdMap = new Dictionary<string, WebSocketClientSessionManager>();
            applicationObjectScope.Add(SessionObjects.SessionsMapBySessionId, _sessionForSessionIdMap);

            _serverInstance = this;
        }
        #endregion Constructor

        #region Member Methods

        protected void InstantiateBufferPools(int maxMessageSize)
        {
            //TODO: 
        }

        public void ProcessRead(Object sessionToken, SelectionKey sk, ByteBuffer bs, int bytesRead)
        {
            if (bytesRead > 0)
            {
                // synchronized
                WebSocketClientSessionManager cm =
                    (WebSocketClientSessionManager) ClientSessionManagerMap.Get(sessionToken);

                if (cm == null)
                {
                    Console.WriteLine("server creating context manager for " + sessionToken);
                    ClientSessionManagerMap.Put(sessionToken, cm);
                }

                // synchronized notify.
            }
        }

        internal void SendUpdateMessage(string receivingSessionId, Messages.UpdateMessage update)
        {
            throw new NotImplementedException();
        }

        protected override void ShutdownImpl()
        {
            Console.WriteLine("Shutdown Impl");
        }

        protected override WebSocketClientSessionManager GenerateContextManager(
            string seesionId, SelectionKey sk, SimplTypesScope translationScope, Scope<object> globalScope) 
        {
            throw new NotImplementedException();
        }

        public void PutServerObject(object o)
        {
            WebSocketServer = (WebSocketServer) o;
        }

        public string GetAPushFromWebSocket(string requestString, string sessionId) 
        {
            Console.WriteLine("Just got GetAPushFromWebSocket: " + requestString);
            ApplicationObjectScope.Add(SessionObjects.SessionId, sessionId);

            WebSocketClientSessionManager theClientSessionManager = null;
            if (_sessionForSessionIdMap.ContainsKey(sessionId))
            {
                theClientSessionManager = _sessionForSessionIdMap.Get(sessionId);
            }
            else
            {
                theClientSessionManager 
                    = new WebSocketClientSessionManager(sessionId, TranslationScope, ApplicationObjectScope);
                _sessionForSessionIdMap.Add(sessionId,theClientSessionManager);
            }

            RequestMessage requestMessage = null;
            try
            {
                requestMessage = theClientSessionManager.TranslateOODSSRequestJSON(requestString);
            }
            catch(Exception e)
            {
            }
            ResponseMessage responseMessage = theClientSessionManager.ProcessRequest(requestMessage);
            StringBuilder pJSON = new StringBuilder();
            SimplTypesScope.Serialize(responseMessage, pJSON, StringFormat.Json);
            return pJSON.ToString();
        }

        public void NewClientAdded(string sessionId)
        {
            Console.WriteLine("New Client Added");
        }

        public bool Invalidate(string sessionId, bool forcePermanent)
        {
            WebSocketClientSessionManager cm = ClientSessionHandleMap.Get(sessionId);

            bool permanent = (forcePermanent ? true : (cm == null ? true : cm.IsInvalidating()));

            if (permanent)
            {
                ClientSessionHandleMap.Remove(sessionId);
                ClientSessionHandleMap.Remove(sessionId);
            }

            if (cm != null)
            {
                cm.Shutdown();
            }

            return forcePermanent;
        }

        #endregion Member Methods
    }
}
