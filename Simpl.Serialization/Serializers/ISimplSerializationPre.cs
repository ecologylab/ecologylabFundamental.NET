using Simpl.Serialization.Context;
using Simpl.Serialization.Graph;

namespace Simpl.Serialization.Serializers
{
    interface ISimplSerializationPre
    {
        void SerializationPreHook(TranslationContext translationContext);
    }
}
