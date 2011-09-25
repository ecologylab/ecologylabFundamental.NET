using System;
using ecologylab.serialization;

namespace Simpl.Serialization
{
    public interface IDeserializationHookStrategy
    {
        void DeserializationPreHook(Object o, FieldDescriptor fd);

        void DeserializationPostHook(Object o, FieldDescriptor fd);
    }
}
