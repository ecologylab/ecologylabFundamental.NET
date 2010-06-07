using System;
using System.Collections.Generic;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml.library
{
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public Item()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        public Item(String title)
        {
            this.title = title;
        }

        /// <summary>
        /// 
        /// </summary>
        public String Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<String> CategorySet
        {
            get { return categorySet; }
            set { categorySet = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Uri Link
        {
            get { return link; }
            set { link = value; }
        }

    }
}
