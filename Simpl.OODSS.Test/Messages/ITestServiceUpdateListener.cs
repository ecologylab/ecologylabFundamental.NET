using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpl.OODSS.Test.Messages
{
    interface ITestServiceUpdateListener
    {
        void OnReceiveUpdate(TestServiceUpdate response);
    }
}
