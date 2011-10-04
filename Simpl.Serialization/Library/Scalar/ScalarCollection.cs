using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Scalar
{
    public class ScalarCollection
    {
        [SimplCollection("int")] 
        private List<Int32> collectionOfIntegers;

        public ScalarCollection()
        {
            collectionOfIntegers = new List<Int32>();
        }

        public void AddInt(int integer)
        {
            collectionOfIntegers.Add(integer);
        }
    }
}
