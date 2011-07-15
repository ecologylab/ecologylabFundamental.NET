using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamentalTestCases.Maps;
using ecologylab.serialization;
using System.IO;
using ecologylabFundamentalTestCases.Polymorphic;
using ecologylabFundamental.ecologylab.serialization;
using ecologylabFundamentalTestCases.ObjectGraph;

namespace ecologylabFundamentalTestCases
{
    class Program
    {
        static void Main(string[] args)
        {
            //MapTest();            
            //PolyMorphicMapTest();
            //SerialisationTest.Run();
            ObjectGraphTest.RunTests();
        }

        private static void PolyMorphicMapTest()
        {
            Configuration test = new Configuration();

            test.fillDictionary();

            StringBuilder output = new StringBuilder();
            test.serialize(output, Format.XML);
            //test.serializeToXML(output,new TranslationContext());

            Console.WriteLine(output);
            Console.ReadLine();

            // create a writer and open the file
            TextWriter tw = new StreamWriter("dict_output.xml");
            tw.WriteLine(output);
            tw.Close();

            TranslationScope.Get("testScope", typeof(Pref), typeof(PrefDouble));
            Configuration outputData = (Configuration)TranslationScope.Get("test", typeof(Configuration), typeof(Pref), typeof(PrefDouble)).deserialize("dict_output.xml", new TranslationContext(), Format.XML);
            output.Clear();
            outputData.serialize(output, Format.XML);
            //outputData.serializeToXML(output,new TranslationContext());
            Console.WriteLine(output);
            Console.ReadLine();
        }

        private static void MapTest()
        {
            SimplContainer test = new SimplContainer();

            test.fillDictionary();

            StringBuilder output = new StringBuilder();
            test.serialize(output, Format.XML);
            //test.serializeToXML(output,new TranslationContext());

            Console.WriteLine(output);
            Console.ReadLine();

            // create a writer and open the file
            TextWriter tw = new StreamWriter("dict_output.xml");
            tw.WriteLine(output);
            tw.Close();

            SimplContainer outputData = (SimplContainer)TranslationScope.Get("test", typeof(SimplContainer), typeof(SimplData)).deserialize("dict_output.xml", new TranslationContext(), Format.XML);
            output.Clear();
            outputData.serialize(output, Format.XML);
            //outputData.serializeToXML(output,new TranslationContext());
            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}
