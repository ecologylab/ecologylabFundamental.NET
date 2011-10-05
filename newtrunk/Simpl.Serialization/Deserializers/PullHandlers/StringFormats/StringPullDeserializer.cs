using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Deserializers.PullHandlers.StringFormats
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class StringPullDeserializer : PullDeserializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        protected StringPullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext, IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputTranslationScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        protected StringPullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext)
            : base(inputTranslationScope, inputContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public abstract Object Parse(String inputString);
    }
}
