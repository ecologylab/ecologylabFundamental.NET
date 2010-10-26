using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamentalTestCases.Maps;
using ecologylabFundamental.ecologylab.serialization;
using System.IO;
using ecologylabFundamentalTestCases.Polymorphic;

namespace ecologylabFundamentalTestCases
{
    class Program
    {
        static void Main(string[] args)
        {
            //MapTest();

            //PolyMorphicMapTest();
        }

        private static void PolyMorphicMapTest()
        {
            Configuration test = new Configuration();

            test.fillDictionary();

            StringBuilder output = new StringBuilder();
            test.serializeToXML(output);

            Console.WriteLine(output);
            Console.ReadLine();

            // create a writer and open the file
            TextWriter tw = new StreamWriter("dict_output.xml");
            tw.WriteLine(output);
            tw.Close();

            Configuration outputData = (Configuration)TranslationScope.Get("test", typeof(Configuration), typeof(Pref), typeof(PrefDouble)).deserialize("dict_output.xml", Format.XML);
            output.Clear();
            outputData.serializeToXML(output);
            Console.WriteLine(output);
            Console.ReadLine();
        }

        private static void MapTest()
        {
            SimplContainer test = new SimplContainer();

            test.fillDictionary();

            StringBuilder output = new StringBuilder();
            test.serializeToXML(output);

            Console.WriteLine(output);
            Console.ReadLine();

            // create a writer and open the file
            TextWriter tw = new StreamWriter("dict_output.xml");
            tw.WriteLine(output);
            tw.Close();

            SimplContainer outputData = (SimplContainer)TranslationScope.Get("test", typeof(SimplContainer), typeof(SimplData)).deserialize("dict_output.xml", Format.XML);
            output.Clear();
            outputData.serializeToXML(output);
            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}
