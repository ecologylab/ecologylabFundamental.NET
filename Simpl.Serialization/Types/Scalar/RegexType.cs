using System;
using System.Text.RegularExpressions;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    /// <summary>
    ///     Class abstracting C# Float type
    /// </summary>
    class RegexType : ScalarType
    {
        /// <summary>
        ///      Calls the parent constructor for String type
        /// </summary>
        public RegexType()
            : base(typeof(Regex), null, null, null)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns></returns>
        public override object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        { return new Regex(value); }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((Regex) instance).ToString();
        }

        public override bool SimplEquals(object left, object right)
        {
            if (left is Regex && right is Regex)
            {
                return base.GenericSimplEquals<string>((left as Regex).ToString(), (right as Regex).ToString());
            }
            else
            {
                return false;
            }
        }
    }
}
