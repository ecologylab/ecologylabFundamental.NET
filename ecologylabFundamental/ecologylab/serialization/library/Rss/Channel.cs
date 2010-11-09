using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.attributes;

namespace ecologylab.serialization.library
{
    /// <summary>
    /// 
    /// </summary>
    public class Channel : ElementState
    {
        /// <summary>
        /// 
        /// </summary>
        [simpl_scalar]
        [simpl_hints(new Hint[] { Hint.XML_LEAF })]
        private String title;

        /// <summary>
        /// 
        /// </summary>
        [simpl_scalar]
        [simpl_hints(new Hint[] { Hint.XML_LEAF })]
        private String description;

        /// <summary>
        /// 
        /// </summary>
        [simpl_scalar]
        [simpl_hints(new Hint[] { Hint.XML_LEAF })]
        private Uri link;

        [simpl_collection("item")]
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
