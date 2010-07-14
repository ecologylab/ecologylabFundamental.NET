using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.serialization.types.scalar
{
    /// <summary>
    ///     Class abstracting C# Float type
    /// </summary>
    class FloatType : ScalarType
    {
        /// <summary>
        ///      Calls the parent constructor for float type
        /// </summary>
        public FloatType()
            : base(typeof(float))
        { }

        /// <summary>
        ///     Creates and returns an instance of a float type for the supplied type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>float</returns>
        public override Object GetInstance(String value, String[] formatStrings)
        {
            return float.Parse(value);
        }
    }
}
