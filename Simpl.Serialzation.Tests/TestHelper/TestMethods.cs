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
            ClassDescriptor.Serialize(obj, format, sw);

            PrettyPrint.Print(sb.ToString(), format);

            return sb.ToString();
        }

        public static Object TestDeserialization(TranslationScope translationScope, String inputString, StringFormat format)
        {
            Object deserializedObj = translationScope.Deserialize(inputString, format);
            return deserializedObj;
        }

        public static void TestSimplObject(Object obj, TranslationScope translationScope, StringFormat format)
        {
            Console.WriteLine("Serializing object " + obj); Console.WriteLine();
            String serializedData = TestSerialization(obj, format);

            Console.WriteLine();
            Object deserializedObj = TestDeserialization(translationScope, serializedData, format);

            Console.WriteLine("Deserialized object " + deserializedObj); Console.WriteLine();
            TestSerialization(deserializedObj, format);
        }

         public static void TestSimplObject(Object obj, TranslationScope translationScope)
         {
             TestSimplObject(obj, translationScope, StringFormat.Xml);
         }
    }
}
