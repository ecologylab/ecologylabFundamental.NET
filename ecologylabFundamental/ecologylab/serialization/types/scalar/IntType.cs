using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylab.serialization.types.scalar
{
    /// <summary>
    ///     Class abstracting C# int type
    /// </summary>
    class IntType : ScalarType
    {

        public static int DEFAULT_VALUE = 0;
        public static String DEFAULT_VALUE_STRING = "0";

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
        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return int.Parse(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return (Int32)field.GetValue(context) == DEFAULT_VALUE;
        }
    }
}
