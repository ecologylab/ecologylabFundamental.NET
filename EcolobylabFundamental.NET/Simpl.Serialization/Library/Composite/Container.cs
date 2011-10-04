using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Composite
{
    public class Container
    {
        [SimplComposite]
        [SimplClasses(new[] { typeof(WcBase), typeof(WcSubOne), typeof(WcSubTwo) })]
        [SimplWrap]
        private WcBase wc;

        public Container()
        {
            
        }

        public Container(WcBase wcBase)
        {
            wc = wcBase;
        }

        public Container(WcSubOne wcSubOne)
        {
            wc = wcSubOne;
        }

        public Container(WcSubTwo wcSubTwo)
        {
            wc = wcSubTwo;
        }
    }
}
