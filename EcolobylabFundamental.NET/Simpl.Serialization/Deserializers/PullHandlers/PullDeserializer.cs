using Simpl.Serialization.Context;

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
        private TranslationScope _inputTranslationScope;

        /// <summary>
        /// 
        /// </summary>
        private TranslationContext _inputContext;

        /// <summary>
        /// 
        /// </summary>
        private IDeserializationHookStrategy _deserializationHookStrategy;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        protected PullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext,
                                IDeserializationHookStrategy deserializationHookStrategy)
        {
            _inputTranslationScope = inputTranslationScope;
            _inputContext = inputContext;
            _deserializationHookStrategy = deserializationHookStrategy;
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


    }

}
