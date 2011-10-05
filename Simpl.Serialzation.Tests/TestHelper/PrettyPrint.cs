using System;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Simpl.Serialization;

namespace Simpl.Serialzation.Tests.TestHelper
{
    public static class PrettyPrint
    {
        public static void PrintJson(String json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            Console.WriteLine(JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented));
        }

        public static void PrintXml(String xml)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNodeReader xmlReader = new XmlNodeReader(doc);
                XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter)
                                              {
                                                  Formatting = System.Xml.Formatting.Indented,
                                                  Indentation = 1,
                                                  IndentChar = '\t'
                                              };
                xmlWriter.WriteNode(xmlReader, true);
                Console.WriteLine(stringWriter.ToString());
            }
        }

        public static void Print(String serializedData, StringFormat format)
        {
            switch (format)
            {
                case StringFormat.Xml:
                    PrintXml(serializedData);
                    break;
                case StringFormat.Json:
                    PrintJson(serializedData);
                    break;
                default:
                    Console.WriteLine(serializedData);
                    break;
            }
        }
    }
}