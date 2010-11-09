using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylab.serialization.types.scalar
{
   
    /// <summary>
    ///     Class abstracting C# long type
    /// </summary>
    class LongType : ScalarType
    {
        /// <summary>
        /// 
        /// </summary>
        public static long DEFAULT_VALUE = 0;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static String DEFAULT_VALUE_STRING = "0";

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

        public override bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return (long)field.GetValue(context) == DEFAULT_VALUE;
        }
    }
}
