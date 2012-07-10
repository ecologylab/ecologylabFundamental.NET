using System;
using System.Reflection;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
   
    /// <summary>
    ///     Class abstracting C# long type
    /// </summary>
    class LongType : ScalarType
    {
        /// <summary>
        /// 
        /// </summary>
        public const long DEFAULT_VALUE = 0;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public const String DEFAULT_VALUE_STRING = "0";

         /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public LongType()
            : this(typeof (long))
        {
        }

        public LongType(Type type)
            : base(type, CLTypeConstants.JavaLong, CLTypeConstants.ObjCLong, null)
        {
        }

        /// <summary>
        ///     Creates and returns an instance of long type for the given
        ///     input value. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>int</returns>
        public override object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return long.Parse(value);
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((long) instance).ToString();
        }

        public override bool IsDefaultValue(FieldInfo field, Object context)
        {
            return (long)field.GetValue(context) == DEFAULT_VALUE;
        }
    }
}
