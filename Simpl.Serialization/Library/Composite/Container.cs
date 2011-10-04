using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization.Library.Composite
{
    class Container
    {
        private WcBase wc;

        public Container()
        {
            
        }

        public Container(WcSubOne wcSubOne)
        {
            this.wc = wcSubOne;
        }
    }
}
