using System;
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
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        protected StringPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext, IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputSimplTypesScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        protected StringPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext)
            : base(inputSimplTypesScope, inputContext)
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
