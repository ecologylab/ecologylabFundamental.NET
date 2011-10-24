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
            StreamWriter sw = new StreamWriter(stream, Encoding.Default);
            Serialize(obj, sw, translationContext);
            sw.Close();
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
