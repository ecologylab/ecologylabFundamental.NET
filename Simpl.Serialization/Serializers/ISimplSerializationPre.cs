using Simpl.Serialization.Context;
using Simpl.Serialization.Graph;

namespace Simpl.Serialization.Serializers
{
    public interface ISimplSerializationPre
    {
        void SerializationPreHook(TranslationContext translationContext);
    }
}
