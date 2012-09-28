using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Serializers.StringFormats
{
    /// <summary>
    /// abstract base class for string serializers. used for preparing input data to serialize into multiple possible data outputs. 
    /// </summary>
    public abstract class StringSerializer : FormatSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="translationContext"></param>
        /// <returns></returns>
        public String Serialize(object obj, TranslationContext translationContext)
        {
            StringBuilder sb = new StringBuilder();
            Serialize(obj, sb, translationContext);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stringBuilder"></param>
        /// <param name="translationContext"></param>
        public void Serialize(object obj, StringBuilder stringBuilder, TranslationContext translationContext)
        {
            Serialize(obj, new StringWriter(stringBuilder), translationContext);
        }
       
        /// <summary>
        /// closes the stream after serializing data. 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="translationContext"></param>
        public override void Serialize(object obj, Stream stream, TranslationContext translationContext)
        {
            StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
            Serialize(obj, sw, translationContext);
            sw.Dispose();
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