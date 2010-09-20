using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.serialization;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamentalTestCases.Polymorphic
{
    public class PrefDouble : IMappable // : Pref
    {
        [simpl_scalar]
        double value;

        [simpl_scalar]
        public String name;

        public PrefDouble() { }

        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public object key()
        {
            return name;
        }

    }
}
