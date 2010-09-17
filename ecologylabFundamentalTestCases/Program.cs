using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamentalTestCases.Maps;
using ecologylabFundamental.ecologylab.serialization;
using System.IO;

namespace ecologylabFundamentalTestCases
{
    class Program
    {
        static void Main(string[] args)
        {
            SimplContainer test = new SimplContainer();

            test.fillDictionary();


            StringBuilder output = new StringBuilder();
            test.serialize(output);

            Console.WriteLine(output);
            Console.ReadLine();


            // create a writer and open the file
            TextWriter tw = new StreamWriter("dict_output.xml");
            tw.WriteLine(output);
            tw.Close();

            SimplContainer outputData = (SimplContainer)TranslationScope.Get("test", typeof(SimplContainer), typeof(SimplData)).deserialize("dict_output.xml");
            output.Clear();
            outputData.serialize(output);
            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}
