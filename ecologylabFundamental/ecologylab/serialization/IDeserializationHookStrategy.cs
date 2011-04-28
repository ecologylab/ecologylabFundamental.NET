using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;

namespace ecologylab.serialization
{
    public interface IDeserializationHookStrategy
    {
        void deserializationPreHook(ElementState e, FieldDescriptor fd);

        void deserializationPostHook(ElementState e, FieldDescriptor fd);
    }
}
