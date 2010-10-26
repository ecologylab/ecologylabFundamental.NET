using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.serialization.json
{
    interface IJSONContentHandler
    {
        void StartJSON();
        void StartObject();
        void StartObjectEntry(String key);
        void StartArray();

        void Primitive(Object value);

        void EndJSON();
        void EndObject();
        void EndObjectEntry();
        void EndArray();
    }
}
