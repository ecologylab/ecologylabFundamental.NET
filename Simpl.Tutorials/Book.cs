using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Serialization.Attributes;
using Simpl.Serialization;


namespace Simpl.Tutorials
{
    public class Book
    {
        [SimplScalar]
        String title;

        [SimplScalar]
        String authorName;

        [SimplScalar]
        [SimplTag("book_number")]
        int bookID;
        //We're going to map bookID to "book_number"
        //These sorts of mappings can be handy, especially for handling old/pre-existing data schemas.
        // [simpl_other_tags()] is also a great tool for this



        public Book()
        {
            /* If we had a field like:
             * [simple_collection] private List<int>() myList;
             *  We would initialize it here like so: */
            // myList = new ArrayList<Integer>();
            // To select our default implementation behind the interface.
            // In this case, we don't have to initialize anything


        }
        public void setAuthorName(String newName)
        {
            authorName = newName;
        }

        public void setTitle(String newTitle)
        {
            title = newTitle;
        }

        public void setBookID(int newID)
        {
            bookID = newID;
        }

        public void initBook()
        {
            this.setAuthorName("Michael Feathers");
            this.setBookID(1337);
            this.setTitle("Working Effectively with Legacy Code");
        }

        public String getTitle()
        {
            return title;
        }
        public String getAuthor()
        {
            return authorName;
        }
        public int getBookID()
        {
            return bookID;
        }
    }
}
