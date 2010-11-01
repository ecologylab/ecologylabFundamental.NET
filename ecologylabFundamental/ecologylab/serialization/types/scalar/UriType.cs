using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.serialization.types.scalar
{
    class UriType : ReferenceType
    {
        public UriType()
            : base(typeof(Uri))
        {
        }

        public override Object GetInstance(String value, String[] formatStrings)
        {
            return new Uri(value);
        } 
    }
}
