using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Net;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Rss
{
    public class Item
    {
        [SimplScalar]
        [SimplHints(new[] { Hint.XmlLeaf })]
        private String title;

        [SimplScalar]
        [SimplHints(new[] { Hint.XmlLeaf })]
        private String description;

//        [SimplScalar]
//        [SimplHints(new[] { Hint.XmlLeaf })]
        private ParsedUri link;

        [SimplScalar]
        [SimplHints(new[] { Hint.XmlLeaf })]
        private String guid;

        [SimplScalar]
        [SimplHints(new[] { Hint.XmlLeaf })]
        private String author;

        [SimplNoWrap] 
        [SimplCollection("category")] 
        private List<String> categorySet; 

        public Item()
        {
            
        }

        public Item(String pTitle, String pDescription, ParsedUri pLink, String pGuid, String pAuthor, List<String> pCategorySet )
        {
            title = pTitle;
            description = pDescription;
            link = pLink;
            guid = pGuid;
            author = pAuthor;
            categorySet = pCategorySet;
        }
    }
}
