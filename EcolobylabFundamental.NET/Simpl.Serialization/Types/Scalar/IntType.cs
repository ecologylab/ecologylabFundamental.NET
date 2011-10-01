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
        ///     Creates and returns an instance of int type for the given
        ///     input value. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>int</returns>
        public override Object GetInstance(String value, String[] formatStrings,
                                           IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return int.Parse(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return (Int32) field.GetValue(context) == defaultValue;
        }
    }
}
