using Simpl.Serialization.Context;
using Simpl.Serialization.Graph;

namespace Simpl.Serialization.Deserializers
{
    interface ISimplDeserializationPost
    {
        void DeserializationPostHook(TranslationContext translationContext);
    }
}
