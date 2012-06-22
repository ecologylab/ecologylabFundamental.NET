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
    public class WebSocketClientSessionManager: BaseSessionManager
    {
        public WebSocketClientSessionManager(string sessionId, ServerProcessor frontend, Scope<object> baseScope)
            : base(sessionId, frontend, baseScope)
        {
        }

        public WebSocketClientSessionManager(string seesionId, SimplTypesScope translationScope, Scope<object> applicationObjectScope)
            : base(seesionId, translationScope , applicationObjectScope)
        {
        }

        protected ResponseMessage PerformService(RequestMessage requestMessage)
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

        public ResponseMessage ProcessRequest(RequestMessage requestMessage)
        {
            LastActivity = DateTime.Now.Ticks;

            ResponseMessage response = null;

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

        public override void SendUpdateToClient(UpdateMessage update)
        {
            throw new NotImplementedException();
        }

        public void SendUpdateToClient(UpdateMessage update, string receivingSessionId)
        {
            Console.WriteLine("Send Update Message Please");
            WebSocketOODSSServer server = (WebSocketOODSSServer) LocalScope.Get(SessionObjects.WebSocketOODSSServer);
            server.SendUpdateMessage(receivingSessionId, update);
        }

        public RequestMessage TranslateOODSSRequestJSON(string messageCharSequence)
        {
            RequestMessage requestMessage =
                (RequestMessage) TranslationScope.Deserialize(messageCharSequence, StringFormat.Json);

            return requestMessage;
        }

        public override IPEndPoint GetAddress()
        {
            throw new NotImplementedException();
        }
    }
}
