using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization.Attributes;
using Simpl.Serialization;

namespace Simpl.Tutorials.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            
            Book abook = new Book();
            //Here's an instance of our type to serialize

            abook.initBook();
            // 
            //

            SimplTypesScope book_example_sts = new SimplTypesScope("book_example", typeof(Book));

            //Serialize to JSON
            String jsonResult = SimplTypesScope.Serialize(abook, StringFormat.Json);

            //Serialize to XML
            // (Just change the StringFormat parameter!)
            String xmlResult = SimplTypesScope.Serialize(abook, StringFormat.Xml);

            Object result1 = book_example_sts.Deserialize(jsonResult, StringFormat.Json);
            Book r1 = (Book)result1;

            Object result2 = book_example_sts.Deserialize(xmlResult, StringFormat.Xml);
            Book r2 = (Book)result2;

            Assert.IsTrue(result1.GetType().IsAssignableFrom(abook.GetType()));

            Assert.AreEqual(r1.getAuthor(), abook.getAuthor());
            Assert.AreEqual(r1.getBookID(), abook.getBookID());
            Assert.AreEqual(r1.getTitle(), abook.getTitle());

            Assert.AreEqual(r1.getAuthor(), "Michael Feathers");
            Assert.AreEqual(r1.getBookID(), 1337);
            Assert.AreEqual(r1.getTitle(), "Working Effectively with Legacy Code");

            Assert.AreEqual(r2.getAuthor(), abook.getAuthor());
            Assert.AreEqual(r2.getBookID(), abook.getBookID());
            Assert.AreEqual(r2.getTitle(), abook.getTitle());

            
        }
    }
}
