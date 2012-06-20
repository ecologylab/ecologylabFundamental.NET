using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.Fundamental.Generic;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Distributed.Impl;
using Simpl.OODSS.Distributed.Server.ClientSessionManager;
using Simpl.Serialization;
using SuperWebSocket;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server
{
    class WebSocketOODSSServer<TScope> : AbstractServer<TScope>, IServerMessages 
        where TScope:Scope<Object>
    {
        #region Data Members

        public WebSocketServer WebSocketServer;

        private DictionaryList<object, WebSocketClientSessionManager<TScope,Scope<Object>>>
            ClientSessionManagerMap = new DictionaryList<object, WebSocketClientSessionManager<TScope, Scope<Object>>>();

        private Dictionary<string, WebSocketClientSessionManager<TScope, Scope<Object>>> _sessionForSessionIdMap;

        private DictionaryList<object, SessionHandle> ClientSessionHandleMap = new DictionaryList<object, SessionHandle>();

        private static readonly Encoding EncodedCharSet = NetworkConstants.Charset;

        private static Decoder CharSetDecoder = EncodedCharSet.GetDecoder();

        protected int MaxMessageSize;

        private static WebSocketOODSSServer<TScope> _serverInstance;

        public static WebSocketOODSSServer<TScope> ServerInstance
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
        public WebSocketOODSSServer(SimplTypesScope requestTranslationScope, TScope applicationObjectScope,
			int idleConnectionTimeout, int maxMessageSize)
            :base(0, Dns.GetHostAddresses(Dns.GetHostName()), requestTranslationScope, applicationObjectScope, 
            idleConnectionTimeout, maxMessageSize)
        {
            MaxMessageSize = maxMessageSize + NetworkConstants.MaxHttpHeaderLength;
            TranslationScope = requestTranslationScope;

            applicationObjectScope.Add(SessionObjects.SessionsMap, ClientSessionHandleMap);
            applicationObjectScope.Add(SessionObjects.WebSocketOODSSServer, this);

            InstantiateBufferPools(MaxMessageSize);

            _sessionForSessionIdMap = new Dictionary<string, WebSocketClientSessionManager<TScope, Scope<object>>>();
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
                WebSocketClientSessionManager<TScope, TParentScope> cm =
                    (WebSocketClientSessionManager<TScope, TParentScope>) ClientSessionManagerMap.Get(sessionToken);

                if (cm == null)
                {
                    Console.WriteLine("server creating context manager for " + sessionToken);
                    ClientSessionManagerMap.Put<TParentScope>(sessionToken, cm);
                }

                // synchronized notify.
            }
        }

        internal void SendUpdateMessage(string receivingSessionId, Messages.UpdateMessage<TScope> update)
        {
            throw new NotImplementedException();
        }

        protected override void ShutdownImpl()
        {
            Console.WriteLine("Shutdown Impl");
        }

        protected override WebSocketClientSessionManager<TScope, TParentScope> GenerateContextManager<TParentScope>(
            string seesionId, SelectionKey sk, SimplTypesScope translationScope, Scope<object> globalScope) 
            where TParentScope : Scope<object>
        {
            throw new NotImplementedException();
        }

        public void PutServerObject(object o)
        {
            WebSocketServer = (WebSocketServer) o;
        }

        public string GetAPushFromWebSocket(string requestMessage, string sessionId)
        {
            throw new NotImplementedException();
        }

        public void NewClientAdded(string sessionId)
        {
            Console.WriteLine("New Client Added");
        }

        public bool Invalidate(string sessionId, bool forcePermanent)
        {
            WebSocketClientSessionManager<TScope, TParentScope> cm = ClientSessionHandleMap.Get(sessionId);

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
