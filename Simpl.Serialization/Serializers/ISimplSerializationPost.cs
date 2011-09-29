using Simpl.Serialization.Context;
using Simpl.Serialization.Graph;

namespace Simpl.Serialization.Serializers
{
    interface ISimplSerializationPost
    {
        void SerializationPostHook(TranslationContext translationContext);
    }
}
