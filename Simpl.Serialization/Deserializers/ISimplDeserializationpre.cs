using Simpl.Serialization.Context;
using Simpl.Serialization.Graph;

namespace Simpl.Serialization.Deserializers
{
    interface ISimplDeserializationPre
    {
        void DeserializationPreHook(TranslationContext translationContext);
    }
}
