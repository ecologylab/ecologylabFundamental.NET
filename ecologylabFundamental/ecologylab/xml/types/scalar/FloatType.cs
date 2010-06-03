using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.types.scalar
{
    class FloatType : ScalarType
    {
        public FloatType()
            : base(typeof(float))
        { }

        public override Object GetInstance(String value, String[] formatStrings)
        {
            return float.Parse(value);       
        }
    }
}
