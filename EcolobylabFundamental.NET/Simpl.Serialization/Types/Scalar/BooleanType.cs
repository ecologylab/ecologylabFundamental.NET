using System;
using System.Reflection;
using Simpl.Serialization.Context;
using ecologylab.serialization.types.scalar;

namespace Simpl.Serialization.Types.Scalar
{
    class BooleanType : ReferenceType
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool DEFAULT_VALUE = false;

        /// <summary>
        /// 
        /// </summary>
        public static String DEFAULT_VALUE_STRING = "False";

        /// <summary>
        /// 
        /// </summary>
        public BooleanType()
            : base(typeof(Boolean))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns></returns>
        public override object GetInstance(String value, string[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return Convert.ToBoolean(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return (bool)field.GetValue(context) == DEFAULT_VALUE;
        }
    }
}
