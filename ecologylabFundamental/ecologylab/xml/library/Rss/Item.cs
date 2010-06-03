using System;
using System.Collections.Generic;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml.library
{
    public class Item : ElementState
    {
        [xml_leaf]
        private String title;

        [xml_leaf]
        private String description;

        [xml_leaf]
        private Uri link;

        [xml_collection("category")]
        private List<String> categorySet;

        public Item()
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

        public List<String> CategorySet
        {
            get { return categorySet; }
            set { categorySet = value; }
        }

        public Uri Link
        {
            get { return link; }
            set { link = value; }
        }

    }
}
