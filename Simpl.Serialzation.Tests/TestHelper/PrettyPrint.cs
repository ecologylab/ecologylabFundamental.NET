using System;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Simpl.Serialization;

namespace Simpl.Serialzation.Tests.TestHelper
{
    /// <summary>
    /// Helper class to print out formatted data for readability. (not efficient). 
    /// </summary>
    public static class PrettyPrint
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        public static void PrintJson(String json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            Console.WriteLine(JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void PrintTlv(byte[] data)
        {
            String s = "";

            for (int i = 0; i < data.Length; i++)
            {

                if (data[i] > -1 && data[i] < 16)
                {
                    s = s + "0";
                }
                s = s + String.Format("{0:X}", data[i] & 0xff); 

                if ((i + 1) % 16 == 0)
                {
                    Console.WriteLine(s);
                    s = "";
                }
            }

            if (s.Length != 4)
            {
                Console.WriteLine(s);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializedData"></param>
        /// <param name="format"></param>
        public static void PrintBinary(byte[] serializedData, Format format)
        {
            switch (format)
            {
                case Format.Tlv:
                    PrintTlv(serializedData);
                    break;
                default:
                    Console.WriteLine(serializedData);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializedData"></param>
        /// <param name="format"></param>
        public static void PrintString(String serializedData, Format format)
        {
            switch (format)
            {
                case Format.Xml:
                    PrintXml(serializedData);
                    break;
                case Format.Json:
                    PrintJson(serializedData);
                    break;
                default:
                    Console.WriteLine(serializedData);
                    break; 
            }
        }
    }
}