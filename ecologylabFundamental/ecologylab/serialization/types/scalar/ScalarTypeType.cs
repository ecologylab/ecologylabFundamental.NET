using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.serialization.types.scalar
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
            : base(typeof(ScalarType))
        { }

        /// <summary>
        ///     Creates and returns an instance of int type for the given
        ///     input value. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns>int</returns>
        public override Object GetInstance(String value, String[] formatStrings)
        {
            ScalarType result = null;
            int length = value.Length;
            if ((value != null) && (length > 0))
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

                result = TypeRegistry.GetType(buffy.ToString());
            }
            return result;
        }
    }
}
