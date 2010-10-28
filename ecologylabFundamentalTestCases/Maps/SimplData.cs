using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.serialization;
using ecologylabFundamental.ecologylab.attributes;
namespace ecologylabFundamentalTestCases.Maps
{
    class SimplData : ElementState, IMappable
    {
        public enum Planet
        {
            Mars, Earth, Jupiter
        }

        [simpl_scalar]
        public String itemKey;

        [simpl_scalar]        
        public String testData;

        [simpl_scalar]
        [simpl_hints(new Hint[] { Hint.XML_LEAF })]
        public Planet planet;

        public object key()
        {
            return itemKey;
        }
    }
}
