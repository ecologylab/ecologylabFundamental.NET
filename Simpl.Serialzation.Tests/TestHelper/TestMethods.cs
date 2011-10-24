using System;
using System.IO;
using System.Text;
using Simpl.Serialization;

namespace Simpl.Serialzation.Tests.TestHelper
{
    public static class TestMethods
    {
        public static String TestSerialization(Object obj, StringFormat format)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            SimplTypesScope.Serialize(obj, sw, format);

            PrettyPrint.Print(sb.ToString(), format);

            return sb.ToString();
        }

        public static Object TestDeserialization(SimplTypesScope simplTypesScope, String inputString, StringFormat format)
        {
            Object deserializedObj = simplTypesScope.Deserialize(inputString, format);
            return deserializedObj;
        }

        public static void TestSimplObject(Object obj, SimplTypesScope simplTypesScope, StringFormat format)
        {

            Console.WriteLine("Serializing object " + obj);
            Console.WriteLine("-----------------------------------------------------------------------------");
            String serializedData = TestSerialization(obj, format);

            Console.WriteLine();
            Object deserializedObj = TestDeserialization(simplTypesScope, serializedData, format);

            Console.WriteLine("Deserialized object " + deserializedObj);
            Console.WriteLine("-----------------------------------------------------------------------------");
            TestSerialization(deserializedObj, format);
        }

        public static void TestSimplObject(Object obj, SimplTypesScope simplTypesScope)
         {
             TestSimplObject(obj, simplTypesScope, StringFormat.Xml);
         }
    }
}
