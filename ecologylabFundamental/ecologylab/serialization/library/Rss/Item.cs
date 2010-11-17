using System;
using System.Collections.Generic;
using ecologylab.attributes;
using ecologylab.net;

namespace ecologylab.serialization.library
{
    /// <summary>
    /// 
    /// </summary>
    public class Item : ElementState
    {
        [simpl_scalar]
        [simpl_hints(new Hint[] { Hint.XML_LEAF })]
        private String title;

        [simpl_scalar]
        [simpl_hints(new Hint[] { Hint.XML_LEAF })]
        private String description;

        [simpl_scalar]
        [simpl_hints(new Hint[] { Hint.XML_LEAF })]
        private ParsedUri link;

        [simpl_collection("category")]
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
        public ParsedUri Link
        {
            get { return link; }
            set { link = value; }
        }

    }
}
