using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Simpl.Serialization.Deserializers.PullHandlers.StringFormats
{
    public static class XmlDeserializerHelper
    {
        public static string ReadString(this XmlReader xmlReader)
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
                return xmlReader.ReadElementContentAsString();
            if (xmlReader.NodeType == XmlNodeType.Text && xmlReader.NodeType != XmlNodeType.EndElement)
                try
                {
                    return xmlReader.ReadContentAsString();
                }
                catch (InvalidOperationException e)
                {
                    Debug.WriteLine("Cannot get content at this node because of the wrong node type");
                }
            return null;
        }
    }
}
