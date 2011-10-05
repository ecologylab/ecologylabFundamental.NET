using System;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    /// <summary>
    ///     Class abstracting C# Float type
    /// </summary>
    class StringType : ScalarType
    {
        /// <summary>
        ///      Calls the parent constructor for String type
        /// </summary>
        public StringType()
            : base(typeof(String), CLTypeConstants.JavaString, CLTypeConstants.ObjCString, null)
        { }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="value"></param>
      /// <param name="formatStrings"></param>
      /// <param name="scalarUnmarshallingContext"></param>
      /// <returns></returns>
        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return value; 
        }
    }
}