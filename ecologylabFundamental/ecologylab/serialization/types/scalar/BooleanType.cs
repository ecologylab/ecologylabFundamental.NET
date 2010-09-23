using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.serialization.types.scalar
{
    class BooleanType : ReferenceType
    {
        public BooleanType()
            : base(typeof(Boolean))
        {
        }

        public override object GetInstance(String value, string[] formatStrings)
        {
            return Convert.ToBoolean(value);
        }
    }
}
