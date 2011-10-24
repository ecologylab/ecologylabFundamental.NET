using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Serializers.BinaryFormats
{
    public abstract class BinarySerializer : FormatSerializer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="translationContext"></param>
        public override void Serialize(object obj, Stream stream, TranslationContext translationContext)
        {
            Serialize(obj, new BinaryWriter(stream), translationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        public abstract void Serialize(object obj, BinaryWriter textWriter, TranslationContext translationContext);
    }
}
