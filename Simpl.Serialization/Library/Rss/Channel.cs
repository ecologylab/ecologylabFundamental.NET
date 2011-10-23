using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Net;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Rss
{
    public class Channel
    {
        [SimplNoWrap]
        [SimplCollection("item")] 
        private List<Item> items;

        [SimplScalar]
        [SimplHints(new[] {Hint.XmlLeaf})] 
        private String title;

        [SimplScalar]
        [SimplHints(new[] {Hint.XmlLeaf})] 
        private String description;

        [SimplScalar] 
        [SimplHints(new[] {Hint.XmlLeaf})] 
        private ParsedUri link;

        public Channel()
        {

        }

        public Channel(String pTitle, String pDescription, ParsedUri pLink, List<Item> pItems)
        {
            title = pTitle;
            description = pDescription;
            link = pLink;
            items = pItems;
        }

        public List<Item> Items
        {
            get { return items; }
        }
    }
}
