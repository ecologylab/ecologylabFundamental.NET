using Ecologylab.Collections;
using Simpl.Fundamental.Generic;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Distributed.Server.ClientSessionManager;
using Simpl.OODSS.Messages;
using Simpl.OODSS.TestClientAndMessage.Messages;
using Simpl.Serialization.Attributes;

namespace Simpl.OODSS.TestService.Messages
{
    class TestServiceRequest : RequestMessage
    {
        [SimplScalar]
        private string message;

        public TestServiceRequest()
        {
        }

        public TestServiceRequest(string message)
        {
            this.message = message;
        }

        public override ResponseMessage PerformService(Scope<object> clientSessionScope)
        {
            var sessionManagerMap =
                (DictionaryList<object, WebSocketClientSessionManager>)
                clientSessionScope.Get(SessionObjects.SessionsMap);

            var sessionId = (string)clientSessionScope.Get(SessionObjects.SessionId);

            var update = new TestServiceUpdate(message, sessionId);

            foreach (WebSocketClientSessionManager client in sessionManagerMap.Values)
            {
                client.SendUpdateToClient(update, client.SessionId);
            }

            return new TestServiceResponse(TestServiceConstants.ServicePrefix + message);
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
