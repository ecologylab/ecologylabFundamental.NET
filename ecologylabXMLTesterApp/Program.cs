using System;
using System.Collections.Generic;
using System.Text;
using ecologylabFundamental.ecologylab.xml.library;
using System.IO;
using ecologylabFundamental.ecologylab.xml;
using ecologylabFundamental.ecologylab.xml.library.Schmannel;

namespace ecologylabXMLTesterApp
{
    class Program
    {
        static void Main(string[] args)
        {

            PolymorphicTest();


        }

        private static void PolymorphicTest()
        {
            /**
            * 
            * Simple test case poly-morphic collection 
            *             
            */
            Item item = new Item("t2ec");
            Item schmItem = new SchmItem("cf");
            Item nested = new BItem("nested");

            Schmannel schmannel = new Schmannel();

            schmannel.polyItem = nested;

            schmannel.PolyAdd(item);
            schmannel.PolyAdd(schmItem);

            StringBuilder output = new StringBuilder();

            schmannel.translateToXMLStringBuilder(output);

            // create a writer and open the file
            TextWriter tw = new StreamWriter("polymorphic_output.xml");
            tw.WriteLine(output);
            tw.Close();


            Console.WriteLine(output);
            Console.ReadLine();

            ElementState es = ElementState.translateFromXML("polymorphic_output.xml", SchmannelTranslations.Get());

            output.Clear();
            es.translateToXMLStringBuilder(output);
            Console.WriteLine(output);
            Console.ReadLine();
        }

        private static void MonomorphicTest()
        {
            /**
             * 
             * Simple test case mono-morphic collection 
             *             
             */
            RssState rssState = new RssState();
            Channel channel = new Channel();
            Item item1 = new Item();
            Item item2 = new Item();
            List<String> categorySet = new List<String>();

            rssState.Version = 1.0f;

            channel.Title = "testTile";
            channel.Description = "testDescription";

            categorySet.Add("category1");
            categorySet.Add("category2");

            item1.Title = "testItem1";
            item1.Description = "testItem1Description";
            item1.CategorySet = categorySet;

            item2.Title = "testItem2";
            item2.Description = "testItem2Description";

            List<Item> items = new List<Item>();
            items.Add(item1);
            items.Add(item2);

            channel.Items = items;
            rssState.Channel = channel;

            StringBuilder output = new StringBuilder();

            rssState.translateToXMLStringBuilder(output);

            // create a writer and open the file
            TextWriter tw = new StreamWriter("monomorphic_output.xml");
            tw.WriteLine(output);
            tw.Close();


            Console.WriteLine(output);
            Console.ReadLine();

            ElementState es = ElementState.translateFromXML("monomorphic_output.xml", RssTranslations.Get());

            output.Clear();
            es.translateToXMLStringBuilder(output);
            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}
