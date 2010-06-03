using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml.library
{
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public Channel()
        {
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
        public List<Item> Items
        {
            get { return items; }
            set { items = value; }
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
