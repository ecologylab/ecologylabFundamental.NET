using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylab.serialization.types.scalar
{
    class BooleanType : ReferenceType
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool DEFAULT_VALUE = false;

        /// <summary>
        /// 
        /// </summary>
        public static String DEFAULT_VALUE_STRING = "False";

        public BooleanType()
            : base(typeof(Boolean))
        {
        }

        public override object GetInstance(String value, string[] formatStrings)
        {
            return Convert.ToBoolean(value);
        }

        public override bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return (bool)field.GetValue(context) == DEFAULT_VALUE;
        }
    }
}
