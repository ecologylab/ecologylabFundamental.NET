﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.serialization;
using ecologylabFundamental.ecologylab.attributes;

namespace ecologylabFundamentalTestCases.Polymorphic
{
    public class PrefDouble : Pref // : Pref
    {
        [simpl_scalar]
        public double value;
        
        public PrefDouble() { }

        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public object key()
        {
            return name;
        }
    }
}