using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server.ClientSessionManager
{
    class WebSocketClientSessionManager<TScope,TParentScope> : BaseSessionManager<TScope,TParentScope> 
        where TScope : Scope<object> 
        where TParentScope : Scope<object>
    {
        public WebSocketClientSessionManager(string sessionId, ServerProcessor frontend, TParentScope baseScope)
            : base(sessionId, frontend, baseScope)
        {
        }

        public WebSocketClientSessionManager(string seesionId, SimplTypesScope translationScope, TScope applicationObjectScope)
            : base(seesionId, translationScope , applicationObjectScope)
        {
        }

        protected ResponseMessage<TScope> PerformService(RequestMessage<TScope> requestMessage)
        {
            try
            {
                return requestMessage.PerformService(LocalScope);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public ResponseMessage<TScope> ProcessRequest(RequestMessage<TScope> requestMessage)
        {
            LastActivity = DateTime.Now.Ticks;

            ResponseMessage<TScope> response = null;

            if (requestMessage == null)
            {
                Console.WriteLine("No request.");
            }
            else
            {
                response = PerformService(requestMessage);
            }
            return response;
        }

        public override void SendUpdateToClient(UpdateMessage<TScope> update)
        {
            throw new NotImplementedException();
        }

        public void SendUpdateToClient(UpdateMessage<TScope> update, string receivingSessionId)
        {
            Console.WriteLine("Send Update Message Please");
            WebSocketOODSSServer server = (WebSocketOODSSServer) LocalScope.Get(SessionObjects.WebSocketOODSSServer);
            server.SendUpdateMessage(receivingSessionId, update);
        }

        public RequestMessage<TScope> TranslateOODSSRequestJSON(string messageCharSequence)
        {
            RequestMessage<TScope> requestMessage =
                (RequestMessage<TScope>) TranslationScope.Deserialize(messageCharSequence, StringFormat.Json);

            return requestMessage;
        }

        public override IPEndPoint GetAddress()
        {
            throw new NotImplementedException();
        }
    }
}
