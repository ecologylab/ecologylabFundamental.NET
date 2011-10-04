using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;

namespace Simpl.Serialzation.Tests.TestHelper
{
    public static class PrettyPrint
    {
        public static void PrintJson(String json)
        {
            Console.WriteLine(json);
        }

        public static void PrintXml(String xml)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlDocument doc = new XmlDocument();

                //get your document

                doc.LoadXml(xml);

                //create reader and writer

                XmlNodeReader xmlReader = new XmlNodeReader(doc);
                XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter)
                                              {Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t'};


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