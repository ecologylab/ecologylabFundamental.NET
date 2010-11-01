using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.serialization.types.scalar
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
