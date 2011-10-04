using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Composite
{
    public class WcBase
    {
        [SimplScalar] private int x;

        public WcBase()
        {
            
        }

        public WcBase(int pX)
        {
            x = pX;
        }
    }
}
