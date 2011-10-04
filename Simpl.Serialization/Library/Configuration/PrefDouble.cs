using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Configuration
{
    [SimplInherit]
    public class PrefDouble : Pref
    {
        [SimplScalar] private double doubleValue;

        public PrefDouble()
        {
            
        }

        public PrefDouble(String pName, Double pDoubleValue) : base (pName)
        {
            doubleValue = pDoubleValue;
        }
    }
}
