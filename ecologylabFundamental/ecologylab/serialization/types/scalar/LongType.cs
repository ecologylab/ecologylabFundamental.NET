using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.serialization.types.scalar
{
   
    /// <summary>
    ///     Class abstracting C# long type
    /// </summary>
    class LongType : ScalarType
    {
        /// <summary>
        ///      Calls the parent constructor for long type
        /// </summary>
        public LongType()
            : base(typeof(long))
        { }

        /// <summary>
        ///     Creates and returns an instance of long type for the given
        ///     input value. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>int</returns>
        public override Object GetInstance(String value, String[] formatStrings)
        {
            return long.Parse(value);
        }
    }
}
