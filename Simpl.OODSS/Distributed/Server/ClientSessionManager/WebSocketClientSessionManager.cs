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

        public WebSocketClientSessionManager(string sessionId, SimplTypesScope translationScope, Scope<object> applicationObjectScope, ServerProcessor frontend)
            : base(sessionId, translationScope , applicationObjectScope, frontend)
        {
        }

        /// <summary>
        /// process request message in serialized form, and return the response message
        /// </summary>
        /// <param name="requestMessageString">request message in serialized form</param>
        /// <param name="uid">request message's uid</param>
        /// <returns>response message</returns>
        public virtual ResponseMessage ProcessString(string requestMessageString, long uid)
        {
            ResponseMessage responseMessage = null;
            var requestMessage = TranslationScope.Deserialize(requestMessageString, StringFormat.Xml);
            if (requestMessage is RequestMessage)
            {
                responseMessage = ProcessRequest((RequestMessage)requestMessage);
            }
            return responseMessage;
        }

        /// <summary>
        /// process the deserialized request message
        /// </summary>
        /// <param name="requestMessage">deserialized request message</param>
        /// <returns>response message</returns>
        protected ResponseMessage ProcessRequest(RequestMessage requestMessage)
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

        /// <summary>
        /// perform service that specified in the request message
        /// </summary>
        /// <param name="requestMessage">request message</param>
        /// <returns>response message</returns>
        protected virtual ResponseMessage PerformService(RequestMessage requestMessage)
        {
            return requestMessage.PerformService(LocalScope);
        }

        //public override void SendUpdateToClient(UpdateMessage update)
        //{
        //    Console.WriteLine("Send update...");
        //    WebSocketOODSSServer server = (WebSocketOODSSServer) LocalScope.Get(SessionObjects.WebSocketOODSSServer);
        //}

        /// <summary>
        /// send update message to client
        /// </summary>
        /// <param name="update">update message</param>
        /// <param name="receivingSessionId">sessionId of the client</param>
        public override void SendUpdateToClient(UpdateMessage update, string receivingSessionId)
        {
            Console.WriteLine("Send Update Message Please");
            WebSocketOODSSServer server = (WebSocketOODSSServer) LocalScope.Get(SessionObjects.WebSocketOODSSServer);
            server.SendUpdateMessage(receivingSessionId, update);
        }

//        public RequestMessage TranslateOODSSRequest(string messageCharSequence)
//        {
//            RequestMessage requestMessage =
//                (RequestMessage) TranslationScope.Deserialize(messageCharSequence, StringFormat.Xml);
//
//            return requestMessage;
//        }

        //public override IPEndPoint GetAddress()
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Get the client's Ip address
        /// </summary>
        /// <returns></returns>
        public override IPEndPoint GetAddress()
        {
            throw new NotImplementedException();
        }
    }
}
