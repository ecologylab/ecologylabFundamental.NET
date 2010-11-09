using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylab.attributes;

namespace ecologylabFundamentalTestCases.Polymorphic
{
    [simpl_inherit]
    public class PrefDouble : Pref
    {
        [simpl_scalar]
        public double value;
        
        public PrefDouble() { }

        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
