using Simpl.Serialization.Context;
using Simpl.Serialization.Graph;

namespace Simpl.Serialization.Serializers
{
    public interface ISimplSerializationPost
    {
        void SerializationPostHook(TranslationContext translationContext);
    }
}
