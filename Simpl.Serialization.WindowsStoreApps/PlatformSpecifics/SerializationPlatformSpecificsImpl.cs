using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Simpl.Serialization.Types;
using Windows.Storage;

namespace Simpl.Serialization.PlatformSpecifics
{
    class SerializationPlatformSpecificsImpl : ISerializationPlatformSpecifics
    {
        public XmlReader CreateReader(TextReader textReader)
        {
            return XmlReader.Create(textReader);
        }

        public void InitializePlatformSpecificTypes()
        {
            new PlatformSpecificTypesWindowsStoreApps();
        }
    }
}
