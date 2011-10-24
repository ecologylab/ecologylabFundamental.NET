using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Fundamental.Net;
using Simpl.Serialization;
using Simpl.Serialization.Library.Rss;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class RssTest
    {
        [TestMethod]
        public void RssTestXml()
        {
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("rss", typeof (Rss),
                                                                     typeof (Channel),
                                                                     typeof (Item));


            List<String> categorySet = new List<String> {"category1", "category2"};

            Item item1 = new Item("testItem1", "testItem1Description", new ParsedUri("http://www.google.com"), "asdf",
                                  "nabeel", categorySet);
            Item item2 = new Item("testItem2", "testItem2Description", new ParsedUri("http://www.google.com"), "asdf",
                                  "nabeel", categorySet);

            Channel c = new Channel("testTile", "testDesc", new ParsedUri("http://www.google.com"),
                                    new List<Item> {item1, item2});

            Rss rss = new Rss(1.4f, c);


            TestMethods.TestSimplObject(rss, simplTypesScope);
        }

        [TestMethod]
        public void RssTestJson()
        {
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("rss", typeof(Rss),
                                                                     typeof(Channel),
                                                                     typeof(Item));


            List<String> categorySet = new List<String> { "cate\\dgory1", "category2" };

            Item item1 = new Item("testItem1", "testItem1Description", new ParsedUri("http://www.google.com"), "asdf",
                                  "nabeel", categorySet);
            Item item2 = new Item("testItem2", "testItem2Description", new ParsedUri("http://www.google.com"), "asdf",
                                  "nabeel", categorySet);

            Channel c = new Channel("testTile", "testDesc", new ParsedUri("http://www.google.com"),
                                    new List<Item> { item1, item2 });

            Rss rss = new Rss(1.4f, c);


            TestMethods.TestSimplObject(rss, simplTypesScope, Format.Json);
        }
    }
}
