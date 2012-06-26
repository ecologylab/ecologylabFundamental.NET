using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using SuperWebSocket;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server.ClientSessionManager
{
    public class WebSocketClientSessionManager: BaseSessionManager
    {

        public WebSocketSession Session { get; set; }

        public WebSocketClientSessionManager(string seesionId, SimplTypesScope translationScope, Scope<object> applicationObjectScope, ServerProcessor frontend)
            : base(seesionId, translationScope , applicationObjectScope, frontend)
        {
        }

        public ResponseMessage ProcessString(string CurrentMessage, long uid)
        {
            ResponseMessage responseMessage = null;
            var requestMessage = TranslationScope.Deserialize(CurrentMessage, StringFormat.Xml);
            if (requestMessage is RequestMessage)
            {
                responseMessage = ProcessRequest((RequestMessage)requestMessage);
            }
            return responseMessage;
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
                if (!Initialized)
                {
                    // special processing for InitConnectionRequest
                    if (requestMessage is InitConnectionRequest)
                    {
                        string incomingSessionId = ((InitConnectionRequest)requestMessage).SessionId;

                        if (incomingSessionId == null)
                        {
                            // client is not expecting an old ContextManager
                            response = new InitConnectionResponse(SessionId);
                        }
                        else
                        {
                            // client is expecting an old ContextManager
                            response = FrontEnd.RestoreContextManagerFromSessionId(incomingSessionId, this) ? new InitConnectionResponse(incomingSessionId) : new InitConnectionResponse(SessionId);
                        }

                        Initialized = true;
                    }
                }
                else
                {
                    response = PerformService(requestMessage);
                }
            }

            return response;
        }

        protected ResponseMessage PerformService(RequestMessage requestMessage)
        {
            return requestMessage.PerformService(LocalScope);
        }

        //public override void SendUpdateToClient(UpdateMessage update)
        //{
        //    Console.WriteLine("Send update...");
        //    WebSocketOODSSServer server = (WebSocketOODSSServer) LocalScope.Get(SessionObjects.WebSocketOODSSServer);
        //}

        public void SendUpdateToClient(UpdateMessage update, string receivingSessionId)
        {
            Console.WriteLine("Send Update Message Please");
            WebSocketOODSSServer server = (WebSocketOODSSServer) LocalScope.Get(SessionObjects.WebSocketOODSSServer);
            server.SendUpdateMessage(receivingSessionId, update);
        }

        public RequestMessage TranslateOODSSRequest(string messageCharSequence)
        {
            RequestMessage requestMessage =
                (RequestMessage) TranslationScope.Deserialize(messageCharSequence, StringFormat.Xml);

            return requestMessage;
        }

        //public override IPEndPoint GetAddress()
        //{
        //    throw new NotImplementedException();
        //}


    }
}
