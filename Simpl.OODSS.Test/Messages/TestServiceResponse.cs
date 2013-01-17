using Simpl.OODSS.Messages;
using Simpl.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpl.OODSS.Test.Messages
{
    class TestServiceResponse : ResponseMessage
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
