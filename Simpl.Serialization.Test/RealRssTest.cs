using System;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Fundamental.Net;
using Simpl.Serialization;
using Simpl.Serialization.Library.Rss;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class RealRssTest
    {
        [TestMethod]
        public void RealRssXml()
        {
            /*
		     * Read in RSS feed from URL
		     */
            const string url = "http://rss.cnn.com/rss/cnn_us.rss";
            WebRequest objRequest = WebRequest.Create(url);
            Stream responseStream = objRequest.GetResponse().GetResponseStream();

            if (responseStream != null)
            {
                String rssContent = new StreamReader(responseStream).ReadToEnd();

                PrintMessage("Raw RSS Feed:");
                PrintXmlData(rssContent);


                /*
		         * Get the simpl types scope. This references all of the classes that we
		         * are considering for translation.
		         */
                SimplTypesScope rssScope = SimplTypesScope.Get("rss", typeof (Rss), typeof (Channel), typeof (Item));

                /*
		         * Instantiate Rss by translating the xml to C# objects. Take a
		         * look at Rss, Channel, and Item to see how they are annotated to
		         * facilitate translation. Notice that Rss's class tag is rss. This is
		         * an inherent rule: all classes that subclass ElementState have a class
		         * tag of just the class name. Normally Simpl.Serialization uses a camel-case
		         * translatio. Fields that are translated into attributes
		         * and sub elements use a similar convention for determining identifiers.
		         */ 
                Rss feed = (Rss) rssScope.Deserialize(rssContent, StringFormat.Xml);

                /*
		         * Notice that, translated back to xml, not all attributes and elements
		         * still remain. If an attribute or element is not annotated in the
		         * corresponding class it is simply ignored.
		         */
                PrintMessage("Feed Translated back to XML by Simpl.Serializaion");
                TestHelper.TestMethods.TestSerialization(feed, Format.Xml);

                /*
		         * Create our own item to add to the channel
		         */
                Item ecologylabItem = new Item
                                          {
                                              Title = "The Interface Ecology Lab",
                                              Description = "Highlights the cool research going on at the lab.",
                                              Author = "Dr. Andruid Kerne",
                                              Link = new ParsedUri("http://www.ecologylab.net")
                                          };

                /*
		         * Add our item to the front of the channel.
		         */
                feed.Channel.Items.Insert(0, ecologylabItem);

                PrintMessage("Feed Translated back to XML with our added item");
                TestHelper.TestMethods.TestSerialization(feed, Format.Xml);
            }
        }

        private void PrintMessage(String message)
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------");
            Console.WriteLine(message);
            Console.WriteLine("--------------------------------------------------------------------------------------------");
        }

        private void PrintXmlData(string xmlData)
        {
            TestHelper.PrettyPrint.PrintXml(xmlData);
            Console.WriteLine();
        }
    }
}
