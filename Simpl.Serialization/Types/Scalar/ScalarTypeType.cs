using System;
using System.Text;
using Simpl.Serialization.Context;
using Simpl.Serialization.Types;

namespace Simpl.Serialization.Types.Scalar
{
    /// <summary>
    ///     Class abstracting C# int type
    /// </summary>
    class ScalarTypeType : ReferenceType
    {
        /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public ScalarTypeType()
            : base(typeof(ScalarType), CLTypeConstants.JavaScalarType, CLTypeConstants.ObjCScalarType, null)
        { }

        /// <summary>
        ///     Creates and returns an instance of int type for the given
        ///     input value. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns>int</returns>
        public override object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            object result = null;
            int length = value.Length;
            if ((length > 0))
            {
                char firstChar = value[0];
                StringBuilder buffy = new StringBuilder(length + 4);	// includes room for "Type"
                if (char.IsLower(firstChar))
                {
                    buffy.Append(char.ToUpper(firstChar));
                    if (length > 1)
                        buffy.Append(value, 1, length - 1);
                }
                else
                    buffy.Append(value);
                buffy.Append("Type");

                result = TypeRegistry.GetScalarTypeBySimpleName(buffy.ToString());
            }
            return result;
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((ScalarType)instance).ToString();
        }
    }
}
