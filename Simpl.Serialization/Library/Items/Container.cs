using System.Collections.Generic;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Items
{
    public class Container
    {
        [SimplNoWrap] 
        [SimplScope("itemScope1")] 
        [SimplCollection] 
        private List<ItemBase> itemCollection1;

        [SimplWrap]
        [SimplScope("itemScope2")]
        [SimplCollection] 
        private List<ItemBase> itemCollection2;

        public Container()
        {

        }

        public void PopulateContainer()
        {
            itemCollection1 = new List<ItemBase>();
            itemCollection2 = new List<ItemBase>();

            itemCollection1.Add(new ItemOne(1, 1));
            itemCollection1.Add(new ItemOne(1, 2));
            itemCollection1.Add(new ItemOne(1, 3));
            itemCollection1.Add(new ItemTwo("one", 1));
            itemCollection1.Add(new ItemTwo("two", 2));
            itemCollection1.Add(new ItemTwo("three", 3));

            itemCollection2.Add(new ItemTwo("one", 1));
            itemCollection2.Add(new ItemTwo("two", 2));
            itemCollection2.Add(new ItemTwo("three", 3));
            itemCollection2.Add(new ItemRandom("four", 4));
            itemCollection2.Add(new ItemRandom("five", 5));
            itemCollection2.Add(new ItemRandom("six", 6));
        }
    }
}
