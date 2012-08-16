namespace Simpl.Serialization.Types.Scalar
{
    using System;
    using System.Reflection;
    using Simpl.Serialization.Context;

    /// <summary>
    /// Represents a scalar value of True or False
    /// </summary>
    class BooleanType : ScalarType
    {
        /// <summary>
        /// The default value for a BooleanType, False
        /// </summary>
        public const bool DefaultValue = false;

        /// <summary>
        /// The default value, represented as a string
        /// </summary>
        public const String DEFAULT_VALUE_STRING = "False";

        /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public BooleanType()
            : this(typeof (Boolean))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public BooleanType(Type type)
            : base(type, CLTypeConstants.JavaBoolean, CLTypeConstants.ObjCBoolean, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns></returns>
        public override object GetInstance(string value, string[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return Convert.ToBoolean(value);
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((bool) instance).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, Object context)
        {
            return (bool) field.GetValue(context) == DefaultValue;
        } 
    }
}
