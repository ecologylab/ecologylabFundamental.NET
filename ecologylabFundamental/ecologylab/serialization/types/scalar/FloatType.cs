using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylab.serialization.types.scalar
{
    /// <summary>
    ///     Class abstracting C# Float type
    /// </summary>
    class FloatType : ScalarType
    {
        /// <summary>
        /// 
        /// </summary>
        public static float DEFAULT_VALUE = 0f;

        /// <summary>
        /// 
        /// </summary>
        public static String DEFAULT_VALUE_STRING = "0";

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
        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return float.Parse(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return (float)field.GetValue(context) == DEFAULT_VALUE;
        }
    }
}
