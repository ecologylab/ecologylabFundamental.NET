﻿using System;
using System.Reflection;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    /// <summary>
    ///     Class abstracting C# double type.
    /// </summary>
    class DoubleType : ScalarType
    {
        /// <summary>
        /// 
        /// </summary>
        public const double DEFAULT_VALUE = 0;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public const String DEFAULT_VALUE_STRING = "0";
       
        /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public DoubleType()
            : this(typeof(Double))
        {
        }

        public DoubleType(Type type)
            : base(type, CLTypeConstants.JavaDouble, CLTypeConstants.ObjCDouble, null)
        {
        }

        /// <summary>
        ///     Creates and returns and instance of a double type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>double</returns>
        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return Double.Parse(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, Object context)
        {
            return (double)field.GetValue(context) == DEFAULT_VALUE;
        }
    }
}
