using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.serialization.types.scalar
{
    /// <summary>
    ///     Class abstracting C# int type
    /// </summary>
    class IntType : ScalarType
    {
        /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public IntType()
            : base(typeof(int))
        { }

        /// <summary>
        ///     Creates and returns an instance of int type for the given
        ///     input value. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>int</returns>
        public override Object GetInstance(String value, String[] formatStrings)
        {
            return int.Parse(value);
        }
    }
}
