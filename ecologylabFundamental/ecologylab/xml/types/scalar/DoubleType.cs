using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.types.scalar
{
    /// <summary>
    ///     Class abstracting C# double type.
    /// </summary>
    class DoubleType : ScalarType
    {
        /// <summary>
        ///     Calls the parent constructor for Double type
        /// </summary>
        public DoubleType()
            : base(typeof(Double))
        { }

        /// <summary>
        ///     Creates and returns and instance of a double type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>double</returns>
        public override Object GetInstance(String value, String[] formatStrings)
        {
            return Double.Parse(value);
        }
    }
}
