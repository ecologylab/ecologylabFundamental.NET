using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Types.Scalar;

namespace Simpl.Serialization.Types
{
    class FundamentalTypes
    {
        public static ScalarType stringType = new StringType();
        public static ScalarType intType = new IntType();

        static FundamentalTypes()
        {
        }
    }
}
