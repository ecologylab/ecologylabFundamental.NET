using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Items
{
    [SimplTag("item_random")]
    public class ItemRandom : ItemBase
    {
        [SimplScalar] 
        private String randomString;

        public ItemRandom()
        {

        }

        public ItemRandom(String pRandomString, int pVar)
        {
            randomString = pRandomString;
            var = pVar;
        }
    }
}
