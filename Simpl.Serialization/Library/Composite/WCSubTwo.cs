using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Composite
{
    [SimplInherit]
    public class WcSubTwo : WcBase
    {
         [SimplScalar] Boolean myBool;

        public WcSubTwo() : base()
        {
            
        }

        public WcSubTwo(Boolean pMyBool, int pX)
            : base(pX)
        {
            myBool = pMyBool;
        }
    }
}
