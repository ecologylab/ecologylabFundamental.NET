using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.PlatformSpecifics
{
    public interface ISerializationPlatformSpecifics
    {
        XmlReader CreateReader(TextReader textReader);

        void InitializePlatformSpecificTypes();
    }
}
