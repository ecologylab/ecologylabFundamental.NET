using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Configuration
{
    [SimplInherit]
    public class PrefInteger : Pref
    {
        [SimplScalar] private int integerValue;

        public PrefInteger()
        {

        }

        public PrefInteger(String pName, int pIntegerValue)
            : base(pName)
        {
            integerValue = pIntegerValue;
        }
    }
}
