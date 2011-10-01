using System;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    /// <summary>
    ///     Class abstracting C# Float type
    /// </summary>
    class StringBuilderType : ScalarType
    {
        /// <summary>
        ///      Calls the parent constructor for String type
        /// </summary>
        public StringBuilderType()
            : base(typeof(StringBuilder), CLTypeConstants.JavaStringBuilder, CLTypeConstants.ObjCStringBuilder, null)
        { }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="value"></param>
       /// <param name="formatStrings"></param>
       /// <param name="scalarUnmarshallingContext"></param>
       /// <returns></returns>
        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        { return new StringBuilder(value); }
    }
}
