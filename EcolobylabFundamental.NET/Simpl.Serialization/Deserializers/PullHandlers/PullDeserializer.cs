using System;
using Simpl.Serialization.Context;
using Simpl.Serialization.Deserializers.PullHandlers.StringFormats;

namespace Simpl.Serialization.Deserializers.PullHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PullDeserializer
    {
        /// <summary>
        /// 
        /// </summary>
        protected TranslationScope translationScope;

        /// <summary>
        /// 
        /// </summary>
        protected TranslationContext translationContext;

        /// <summary>
        /// 
        /// </summary>
        protected IDeserializationHookStrategy deserializationHookStrategy;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        protected PullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext,
                                IDeserializationHookStrategy deserializationHookStrategy)
        {
            translationScope = inputTranslationScope;
            translationContext = inputContext;
            deserializationHookStrategy = deserializationHookStrategy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        protected PullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext)
            : this(inputTranslationScope, inputContext, null)
        {
        }

        public static StringPullDeserializer GetStringDeserializer(TranslationScope translationScope, TranslationContext translationContext, IDeserializationHookStrategy deserializationHookStrategy, StringFormat format)
        {
            switch (format)
            {
                case StringFormat.Xml:
                    return new XmlPullDeserializer(translationScope, translationContext, deserializationHookStrategy);
                    case StringFormat.Json:
                    return new JsonPullDeserializer(translationScope, translationContext, deserializationHookStrategy);
                    case StringFormat.Bibtex:
                default:
                    throw  new SimplTranslationException(format + "format not supported");
            }
        }



        protected void DeserializationPreHook(Object obj, TranslationContext pTranslationContext)
        {
            if (obj is ISimplDeserializationPre)
            {
                ((ISimplDeserializationPre) obj).DeserializationPreHook(pTranslationContext);
            }
        }
    }
}
