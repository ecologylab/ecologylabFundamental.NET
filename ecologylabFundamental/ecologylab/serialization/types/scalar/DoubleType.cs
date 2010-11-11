using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylab.serialization.types.scalar
{
    /// <summary>
    ///     Class abstracting C# double type.
    /// </summary>
    class DoubleType : ScalarType
    {
        /// <summary>
        /// 
        /// </summary>
        public static double DEFAULT_VALUE = 0;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static String DEFAULT_VALUE_STRING = "0";
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
        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return Double.Parse(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return (double)field.GetValue(context) == DEFAULT_VALUE;
        }
    }
}
