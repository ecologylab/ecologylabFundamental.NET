using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.types.scalar
{
    class StringType : ScalarType
    {
        public StringType()
            : base(typeof(String))
        { }

        public override Object GetInstance(String value, String[] formatStrings)
        { return value; }
    }
}
