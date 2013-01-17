using Ecologylab.Collections;
using Simpl.OODSS.Messages;
using Simpl.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpl.OODSS.Test.Messages
{
    class TestServiceUpdate : UpdateMessage
    {
        [SimplScalar] private string message;

        [SimplScalar] private string id;

        public TestServiceUpdate()
        {
        }

        public TestServiceUpdate(string message, string id)
        {
            this.message = message;
            this.id = id;
        }

        public override void ProcessUpdate(Scope<object> objectRegistry)
        {
            var listener = (ITestServiceUpdateListener) objectRegistry.Get(TestServiceConstants.ServiceUpdateListener);

            if (listener != null)
            {
                listener.OnReceiveUpdate(this);
            }
            else
            {
                throw new NullReferenceException("cannot find client in application scope");
            }
        }

        public string Message { get; set; }
        public string Id { get; set; }

    }
}
