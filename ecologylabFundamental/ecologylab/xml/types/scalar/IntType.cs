using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.types.scalar
{
    class IntType : ScalarType
    {

        public IntType()
            : base(typeof(int))
        { }

        public override Object GetInstance(String value, String[] formatStrings)
        { return null; }
    }
}
