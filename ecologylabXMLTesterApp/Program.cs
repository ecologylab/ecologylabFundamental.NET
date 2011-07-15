using System;
using System.Collections.Generic;
using System.Text;
using ecologylab.attributes;
using ecologylab.serialization.library;
using System.IO;
using ecologylab.serialization;
using ecologylab.serialization.library.Schmannel;
using ecologylabFundamental.ecologylab.serialization;

namespace ecologylabXMLTesterApp
{

    public class SimplTest : ElementState
    {
        [simpl_collection("Some")]
        [xml_tag("Some")]
        public List<SimplItem> simplItems = new List<SimplItem>();
    }

    public class SimplItem : ElementState
    {

        [simpl_collection("Some")]
        public List<SimplInnerItem> simplInnerItems = new List<SimplInnerItem>();
        
    }

    public class SimplInnerItem : ElementState
    {
        [simpl_scalar]
        public String test;
    }

    class Program
    {
        static void Main(string[] args)
        {

            //String jsonString = @"{""circle"":{""radius"":""10"", ""center"":{""x"":""2"", ""y"":""3""}, ""area"":""100""}}";
            //String antherString = "{\"container\":{\"a_objects\":{\"objectsA\":[{\"u\":\"1\", \"w\":\"2\"},{\"u\":\"2\", \"w\":\"4\"},{\"u\":\"3\", \"w\":\"6\"},{\"u\":\"4\", \"w\":\"8\"},{\"u\":\"5\", \"w\":\"10\"},{\"u\":\"6\", \"w\":\"12\"},{\"u\":\"7\", \"w\":\"14\"},{\"u\":\"8\", \"w\":\"16\"},{\"u\":\"9\", \"w\":\"18\"},{\"u\":\"10\", \"w\":\"20\"}]}, \"my_integers\":{\"myints\":[\"10\",\"11\",\"12\",\"13\",\"14\",\"15\",\"16\",\"17\",\"18\",\"19\"]}}}";
            //String oneMoreString = "{\"configuration\":{\"pref_integer\":{\"name\":\"integer_pref\", \"int_value\":\"2\"}, \"prefs\":[{\"pref\":{\"name\":\"only_pref\"}},{\"pref_integer\":{\"name\":\"integer_pref\", \"int_value\":\"2\"}},{\"pref_double\":{\"name\":\"double_pref\", \"double_value\":\"5.0\"}}]}}";
            //ElementStateJSONHandler jsonHandler = new ElementStateJSONHandler();
            ////jsonHandler.parse(oneMoreString);
            //Console.ReadLine();

           // Console.WriteLine("testing polymorphic collection");
            //Console.WriteLine();
            //PolymorphicTest();

            //Console.WriteLine("testing monomorphic colleciton");
            //Console.WriteLine();

            SimplTest test = new SimplTest();

            for (int i = 0; i < 10; i++)
            {
                SimplItem item = new SimplItem();
                for (int j = 0; j < 10; j++)
                {
                    item.simplInnerItems.Add(new SimplInnerItem {test = "j " + j});
                }
                test.simplItems.Add(item);
            }

            var stringBuilder = new StringBuilder();
            //test.serializeToJSON(stringBuilder,new TranslationContext());
            test.serialize(stringBuilder, Format.JSON);

            Console.WriteLine("buffy" + stringBuilder);

            TranslationScope scop = new TranslationScope("chuut", typeof (SimplItem), typeof (SimplTest));

            ElementState deserializeString = scop.deserializeString(stringBuilder.ToString(), new TranslationContext(), Format.JSON);

            Console.WriteLine("Done !");
            //MonomorphicTest();
            Console.Read();
        }

        private static void PolymorphicTest()
        {
            /**
            * 
            * Simple test case poly-morphic collection 
            *             
            */
            Item item = new Item("t2ec\" is an item");
            Item schmItem = new SchmItem("cf");
            Item nested = new BItem("nested");

            Schmannel schmannel = new Schmannel();

            schmannel.polyItem = nested;

            schmannel.PolyAdd(item);
            schmannel.PolyAdd(schmItem);

            StringBuilder output = new StringBuilder();
            StringBuilder jsonOutput = new StringBuilder();

            schmannel.serialize(output, Format.XML);
            //schmannel.serializeToXML(output,new TranslationContext());
            schmannel.serialize(jsonOutput, Format.JSON);
            //schmannel.serializeToJSON(jsonOutput,new TranslationContext());

            // create a writer and open the file
            TextWriter tw = new StreamWriter("polymorphic_output.xml");
            tw.WriteLine(output);
            tw.Close();

            // create a writer and open the file
            TextWriter jtw = new StreamWriter("json_polymorphic_output.xml");
            jtw.WriteLine(jsonOutput);
            jtw.Close();


            Console.WriteLine(output);
            Console.ReadLine();

            Console.WriteLine(jsonOutput);
            Console.ReadLine();

            ElementState es = SchmannelTranslations.Get().deserialize("json_polymorphic_output.xml", new TranslationContext(), Format.JSON);

            output.Clear();
            es.serialize(output, Format.XML);
            Console.WriteLine(output);
            Console.ReadLine();
        }

        private static void MonomorphicTest()
        {
            /**
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
            //channel.Link = new Uri("http://www.google.com");

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
            StringBuilder jsonOutput = new StringBuilder();

            rssState.serialize(output, Format.XML);
            //rssState.serializeToXML(output,new TranslationContext());
            //rssState.serializeToJSON(jsonOutput,new TranslationContext());
            rssState.serialize(jsonOutput, Format.JSON);

            // create a writer and open the file
            TextWriter tw = new StreamWriter("monomorphic_output.xml");
            tw.WriteLine(output);
            tw.Close();

            // create a writer and open the file
            TextWriter jtw = new StreamWriter("json_monomorphic_output.xml");
            jtw.WriteLine(jsonOutput);
            jtw.Close();
            
            Console.WriteLine(output);
            Console.ReadLine();

            Console.WriteLine(jsonOutput);
            Console.ReadLine();

            ElementState es = RssTranslations.Get().deserialize("json_monomorphic_output.xml", new TranslationContext() ,Format.JSON);

            output.Clear();
            es.serialize(output, Format.XML);
            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}