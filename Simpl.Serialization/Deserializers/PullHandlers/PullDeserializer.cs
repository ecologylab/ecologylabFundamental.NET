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
        /// <param name="deserializationHookStrategy"></param>
        protected PullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext,
                                IDeserializationHookStrategy deserializationHookStrategy)
        {
            simplTypesScope = inputSimplTypesScope;
            translationContext = inputContext;
            deserializationHookStrategy = deserializationHookStrategy;
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

        public static StringPullDeserializer GetStringDeserializer(SimplTypesScope simplTypesScope, TranslationContext translationContext, IDeserializationHookStrategy deserializationHookStrategy, StringFormat format)
        {
            switch (format)
            {
                case StringFormat.Xml:
                    return new XmlPullDeserializer(simplTypesScope, translationContext, deserializationHookStrategy);
                    case StringFormat.Json:
                    return new JsonPullDeserializer(simplTypesScope, translationContext, deserializationHookStrategy);
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
