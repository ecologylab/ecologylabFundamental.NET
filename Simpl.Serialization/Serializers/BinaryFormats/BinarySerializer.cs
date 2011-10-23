using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Serializers.BinaryFormats
{
    class BinarySerializer : FormatSerializer
    {
        public override void Serialize(object obj, Stream stream, TranslationContext translationContext)
        {
            throw new NotImplementedException();
        }
    }
}
