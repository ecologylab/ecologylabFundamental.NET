using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml.library
{
    public class Channel : ElementState
    {
        [xml_leaf]
        private String title;

        [xml_leaf]
        private String description;

        [xml_leaf]
        private Uri link;

        [xml_collection("item")]
        private List<Item> items;

        public Channel()
        {
        }

        public String Title
        {
            get { return title; }
            set { title = value; }
        }

        public String Description
        {
            get { return description; }
            set { description = value; }
        }

        public List<Item> Items
        {
            get { return items; }
            set { items = value; }
        }

        public Uri Link
        {
            get { return link; }
            set { link = value; }
        }
    }
}
