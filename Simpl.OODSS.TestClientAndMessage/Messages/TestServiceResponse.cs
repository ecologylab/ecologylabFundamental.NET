using Simpl.OODSS.Messages;
using Simpl.Serialization.Attributes;

namespace Simpl.OODSS.TestClientAndMessage.Messages
{
    public class TestServiceResponse : ResponseMessage
    {
        [SimplScalar] private string message;

        public TestServiceResponse()
        {
        }

        public TestServiceResponse(string message)
        {
            this.message = message;
        }

        public override bool IsOK()
        {
            return true;
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
