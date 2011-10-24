using System;
using System.IO;
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
        protected SimplTypesScope simplTypesScope;

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
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="hookStrategy"></param>
        protected PullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext,
                                   IDeserializationHookStrategy hookStrategy)
        {
            simplTypesScope = inputSimplTypesScope;
            translationContext = inputContext;
            deserializationHookStrategy = hookStrategy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        protected PullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext)
            : this(inputSimplTypesScope, inputContext, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simplTypesScope"></param>
        /// <param name="translationContext"></param>
        /// <param name="hookStrategy"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static StringPullDeserializer GetStringDeserializer(SimplTypesScope simplTypesScope,
                                                                   TranslationContext translationContext,
                                                                   IDeserializationHookStrategy hookStrategy,
                                                                   StringFormat format)
        {
            switch (format)
            {
                case StringFormat.Xml:
                    return new XmlPullDeserializer(simplTypesScope, translationContext, hookStrategy);
                case StringFormat.Json:
                    return new JsonPullDeserializer(simplTypesScope, translationContext, hookStrategy);
                case StringFormat.Bibtex:
                default:
                    throw new SimplTranslationException(format + "format not supported");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public abstract Object Parse(Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pTranslationContext"></param>
        protected void DeserializationPostHook(Object obj, TranslationContext pTranslationContext)
        {
            if (obj is ISimplDeserializationPost)
            {
                ((ISimplDeserializationPost) obj).DeserializationPostHook(pTranslationContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pTranslationContext"></param>
        protected void DeserializationPreHook(Object obj, TranslationContext pTranslationContext)
        {
            if (obj is ISimplDeserializationPre)
            {
                ((ISimplDeserializationPre) obj).DeserializationPreHook(pTranslationContext);
            }
        }
    }
}
