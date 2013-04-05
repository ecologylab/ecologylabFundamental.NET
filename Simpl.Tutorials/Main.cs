using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Serialization.Attributes;
using Simpl.Serialization;


namespace Simpl.Tutorials
{
    class lettersthereain
    {


        public static void Main(string[] args)
        {
            Book abook = new Book();
            //Here's an instance of our type to serialize

            abook.setAuthorName("Michael Feathers");
            abook.setBookID(1337);
            abook.setTitle("Working Effectively with Legacy Code");
            // 
            //
          
            SimplTypesScope book_example_sts = new SimplTypesScope("book_example", typeof(Book));

                //Serialize to JSON
            String jsonResult = SimplTypesScope.Serialize(abook, StringFormat.Json);
            
                //Serialize to XML
            // (Just change the StringFormat parameter!)
            String xmlResult  = SimplTypesScope.Serialize(abook, StringFormat.Xml);

            Object result1 = book_example_sts.Deserialize(jsonResult, StringFormat.Json);
            Book r1 = (Book) result1;

            Object result2 = book_example_sts.Deserialize(xmlResult, StringFormat.Xml);
            Book r2 = (Book) result2;

            Console.WriteLine(jsonResult);

            Console.WriteLine(xmlResult);
            Console.ReadLine();
         }
    }
}
