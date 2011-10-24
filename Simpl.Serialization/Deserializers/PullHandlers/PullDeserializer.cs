using System;
using System.IO;
using Simpl.Serialization.Context;
using Simpl.Serialization.Deserializers.PullHandlers.BinaryFormats;
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
        /// <param name="pSimplTypesScope"></param>
        /// <param name="pTranslationContext"></param>
        /// <param name="pDeserializationHookStrategy"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static StringPullDeserializer GetStringDeserializer(SimplTypesScope pSimplTypesScope, TranslationContext pTranslationContext, IDeserializationHookStrategy pDeserializationHookStrategy, StringFormat format)
        {
            switch (format)
            {
                case StringFormat.Xml:
                    return new XmlPullDeserializer(pSimplTypesScope, pTranslationContext, pDeserializationHookStrategy);
                case StringFormat.Json:
                    return new JsonPullDeserializer(pSimplTypesScope, pTranslationContext, pDeserializationHookStrategy);
                case StringFormat.Bibtex:
                default:
                    throw new SimplTranslationException(format + "format not supported");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSimplTypesScope"></param>
        /// <param name="pTranslationContext"></param>
        /// <param name="pDeserializationHookStrategy"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static PullDeserializer GetPullDeserializer(SimplTypesScope pSimplTypesScope, TranslationContext pTranslationContext, IDeserializationHookStrategy pDeserializationHookStrategy, Format format)
        {
            switch (format)
            {
                case Format.Xml:
                    return new XmlPullDeserializer(pSimplTypesScope, pTranslationContext, pDeserializationHookStrategy);
                case Format.Json:
                    return new JsonPullDeserializer(pSimplTypesScope, pTranslationContext, pDeserializationHookStrategy);
                case Format.Bibtex:
                    throw new SimplTranslationException("bibtex serialization is not supported");
                case Format.Tlv:
                    return new TlvPullDeserializer(pSimplTypesScope, pTranslationContext, pDeserializationHookStrategy);
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
