using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;

namespace Simpl.Serialzation.Tests
{
    public static class TestMethods
    {
        public static String TestSerialization(Object obj)
        {
            return null;
        }

        public static Object TestDeserialization(TranslationScope translationScope, String inputString)
        {
            return null;
        }

        public static void TestSimplObject(Object obj, TranslationScope translationScope)
        {
            Console.WriteLine("Serializing object " + obj);
            String serializedData = TestSerialization(obj);

            Console.WriteLine();
            Object deserializedObj = TestDeserialization(translationScope, serializedData);

            Console.WriteLine("Deserialized object " + deserializedObj);
            String deserializedObjectString = TestSerialization(deserializedObj);
        }
    }
}
