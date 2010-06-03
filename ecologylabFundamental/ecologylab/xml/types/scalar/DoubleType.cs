using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.types.scalar
{
    class DoubleType : ScalarType
    {

        public DoubleType()
            : base(typeof(Double))
        { }

        public override Object GetInstance(String value, String[] formatStrings)
        { return null; }
    }
}
