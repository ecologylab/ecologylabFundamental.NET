using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Items
{
    [SimplInherit]
    public class ItemTwo : ItemBase
    {
        [SimplScalar] 
        private String testString;

        public ItemTwo()
        {

        }

        public ItemTwo(String pTestString, int pVar)
        {
            testString = pTestString;
            var = pVar;
        }
    }
}
