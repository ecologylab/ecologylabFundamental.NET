using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Deserializers.PullHandlers.BinaryFormats
{
    public abstract class BinaryPullDeserializer : PullDeserializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        public BinaryPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext,
                                      IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputSimplTypesScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        public BinaryPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext)
            : base(inputSimplTypesScope, inputContext)
        {
        }

        public abstract Object Parse(BinaryReader binaryWriter);
    }
}
