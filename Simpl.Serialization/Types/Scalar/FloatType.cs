using System;
using System.Reflection;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    /// <summary>
    ///     Class abstracting C# Float type
    /// </summary>
    class FloatType : ScalarType
    {
        /// <summary>
        /// 
        /// </summary>
        public const float DEFAULT_VALUE = 0f;

        /// <summary>
        /// 
        /// </summary>
        public const String DEFAULT_VALUE_STRING = "0";

        /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public FloatType()
            : this(typeof (float))
        {
        }

        public FloatType(Type type)
            : base(type, CLTypeConstants.JavaFloat, CLTypeConstants.ObjCFloat, null)
        {
        }

        /// <summary>
        ///     Creates and returns an instance of a float type for the supplied type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>float</returns>
        public override object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return float.Parse(value);
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((float) instance).ToString();
        }

        /// <summary>
       /// 
       /// </summary>
       /// <param name="field"></param>
       /// <param name="context"></param>
       /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, Object context)
        {
            return (float)field.GetValue(context) == DEFAULT_VALUE;
        }

        public override bool SimplEquals(object left, object right)
        {
            return base.GenericSimplEquals<float>(left, right);
        }
    }
}
