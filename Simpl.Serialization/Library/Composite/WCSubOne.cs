using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Composite
{
    [SimplInherit]
    public class WcSubOne : WcBase
    {
        [SimplScalar] private String subString;

        public WcSubOne() : base()
        {
            
        }

        public WcSubOne(String pSubString, int pX) : base(pX)
        {
            subString = pSubString;
        }
    }
}
