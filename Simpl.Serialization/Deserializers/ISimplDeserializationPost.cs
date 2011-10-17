using Simpl.Serialization.Context;
using Simpl.Serialization.Graph;

namespace Simpl.Serialization.Deserializers
{
    public interface ISimplDeserializationPost
    {
        void DeserializationPostHook(TranslationContext translationContext);
    }
}
