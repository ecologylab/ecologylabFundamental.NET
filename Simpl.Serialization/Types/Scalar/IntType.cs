using System;
using System.Reflection;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    /// <summary>
    ///     Class abstracting C# int type
    /// </summary>
    internal class IntType : ScalarType
    {

        public new const int DefaultValue = 0;
        public new const String DefaultValueString = "0";

        /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public IntType()
            : this(typeof (int))
        {
        }

        public IntType(Type type)
            : base(type, CLTypeConstants.JavaInteger, CLTypeConstants.ObjCInteger, null)
        {
        }



       /// <summary>
       /// 
       /// </summary>
       /// <param name="value"></param>
       /// <param name="formatStrings"></param>
       /// <param name="scalarUnmarshallingContext"></param>
       /// <returns></returns>
        public override object GetInstance(String value, String[] formatStrings,
                                           IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return int.Parse(value);
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((int) instance).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, Object context)
        {
            return (Int32) field.GetValue(context) == DefaultValue;
        }



        public override bool SimplEquals(object left, object right)
        {
            return base.GenericSimplEquals<int>(left, right);
        }
    }
}
