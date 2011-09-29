using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Deserializers.PullHandlers.StringFormats
{
    /// <summary>
    /// 
    /// </summary>
    class XmlPullDeserializer : StringPullDeserializer
    {
        /// <summary>
        /// 
        /// </summary>
        private XmlTextReader _xmlReader; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        public XmlPullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext, IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputTranslationScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        public XmlPullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext)
            : base(inputTranslationScope, inputContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public override Object Parse(String inputString)
        {
            ConfigureInput(inputString);
            return Parse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        private void ConfigureInput(String inputString)
        {
            ConfigureInput(new MemoryStream(Encoding.ASCII.GetBytes(inputString)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        private void ConfigureInput(Stream inputStream)
        {
            _xmlReader = new XmlTextReader(inputStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private object Parse()
        {
            throw new NotImplementedException();
        }
    }
}
