using System;
using System.IO;
using System.Text;
using Simpl.Serialization;

namespace Simpl.Serialzation.Tests.TestHelper
{
    /// <summary>
    /// Helper function to serialize and deserailze objects 
    /// </summary>
    public static class TestMethods
    {
        /// <summary>
        /// serializes data and returns an the serialized data as a stream for application to use. 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Stream TestSerialization(Object obj, Format format)
        {
            HelperStream hStream = new HelperStream();
            SimplTypesScope.Serialize(obj, hStream, format);
            switch (format)
            {
                case Format.Tlv:
                    PrettyPrint.PrintBinary(hStream.BinaryData, format);
                    break;
                default:
                    PrettyPrint.PrintString(hStream.StringData, format);
                    break;
            }
            return new MemoryStream(hStream.BinaryData); 
        }

        /// <summary>
        /// deseiralizes the data, given the input stream and format. returns object representation of the input data. 
        /// </summary>
        /// <param name="simplTypesScope"></param>
        /// <param name="inputStream"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Object TestDeserialization(SimplTypesScope simplTypesScope, Stream inputStream, Format format)
        {
            Object deserializedObj = simplTypesScope.Deserialize(inputStream, format);
            return deserializedObj;
        }

        /// <summary>
        /// Helper methods to test de/serialization of and input object and simpl type scope in particular format
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="simplTypesScope"></param>
        /// <param name="format"></param>
        public static void TestSimplObject(Object obj, SimplTypesScope simplTypesScope, Format format)
        {
            Console.WriteLine("Serializing object " + obj);
            Console.WriteLine("-----------------------------------------------------------------------------");
            Stream outputStream = TestSerialization(obj, format);

            Console.WriteLine();
            Object deserializedObj = TestDeserialization(simplTypesScope, outputStream, format);

            Console.WriteLine("Deserialized object " + deserializedObj);
            Console.WriteLine("-----------------------------------------------------------------------------");
            TestSerialization(deserializedObj, format);
        }

        /// <summary>
        /// simplified overload method to test de/serialization in Xml only.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="simplTypesScope"></param>
        public static void TestSimplObject(Object obj, SimplTypesScope simplTypesScope)
        {
            TestSimplObject(obj, simplTypesScope, Format.Xml);
        }
    }
}
