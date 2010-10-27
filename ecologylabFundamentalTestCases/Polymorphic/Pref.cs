using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.serialization;
using ecologylabFundamental.ecologylab.attributes;

namespace ecologylabFundamentalTestCases.Polymorphic
{
    public class Pref : ElementState, IMappable
    {
        [simpl_scalar]
        public String name;

        public Pref() { }

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
