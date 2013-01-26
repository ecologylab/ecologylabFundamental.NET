using Ecologylab.Collections;
using Simpl.Fundamental.Generic;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Messages;
using Simpl.Serialization.Attributes;

namespace Simpl.OODSS.TestClientAndMessage.Messages
{
    class TestServiceRequest : RequestMessage
    {
        [SimplScalar] private string message;

        public TestServiceRequest()
        {
        }

        public TestServiceRequest(string message)
        {
            this.message = message;
        }

        public override ResponseMessage PerformService(Scope<object> clientSessionScope)
        {
            return null; //do not need implementation on client side
        }

        public string Message 
        { 
            get { return message; }
            set { message = value; }
        }
    }
}
