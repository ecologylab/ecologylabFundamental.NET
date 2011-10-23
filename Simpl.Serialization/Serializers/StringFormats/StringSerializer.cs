using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Serializers.StringFormats
{
    public abstract class StringSerializer : FormatSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        public void Serialize(object obj, TextWriter textWriter)
        {
            Serialize(obj, textWriter, new TranslationContext());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="translationContext"></param>
        public override void Serialize(object obj, Stream stream, TranslationContext translationContext)
        {
            Serialize(obj, new StreamWriter(stream), new TranslationContext());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        public abstract void Serialize(object obj, TextWriter textWriter, TranslationContext translationContext);

       
    }
}
