using System.IO;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Deserializers.PullHandlers.BinaryFormats
{
    /// <summary>
    /// 
    /// </summary>
    public class TlvPullDeserializer : BinaryPullDeserializer
    {
        /// <summary>
        /// 
        /// </summary>
        private BinaryReader _binaryReader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        public TlvPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext,
                                   IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputSimplTypesScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        public TlvPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext)
            : base(inputSimplTypesScope, inputContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override object Parse(Stream stream)
        {
            _binaryReader = new BinaryReader(stream);
            return Parse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBinaryWriter"></param>
        /// <returns></returns>
        public override object Parse(BinaryReader pBinaryWriter)
        {
            _binaryReader = pBinaryWriter;
            return Parse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Parse()
        {
            return null;
        }
    }
}
