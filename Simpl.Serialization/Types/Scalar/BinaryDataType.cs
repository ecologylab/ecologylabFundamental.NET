using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    /// <summary>
    ///     Class abstraction C# binary data type
    /// </summary>
    class BinaryDataType : ScalarType
    {
        public BinaryDataType()
            :this(typeof(MemoryStream))
        {
        }

        public BinaryDataType(Type type)
            : base(type, CLTypeConstants.JavaBinaryData, null, null)
        {
        }

        public override object GetInstance(string value, string[] formatStrings, IScalarUnmarshallingContext unmarshallingContext)
        {
            byte[] bytes = System.Convert.FromBase64String(value);
            return new MemoryStream(bytes);
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            byte[] bytes = ((MemoryStream) instance).ToArray();
            return System.Convert.ToBase64String(bytes);
        }

        public override bool SimplEquals(object left, object right)
        {
            if (left is MemoryStream && right is MemoryStream)
            {
                return Enumerable.SequenceEqual((left as MemoryStream).ToArray(), (right as MemoryStream).ToArray());
            }
            else
            {
                return false;
            }
        }
    }
    
}
