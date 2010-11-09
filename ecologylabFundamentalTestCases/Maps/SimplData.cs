using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylab.attributes;
using ecologylab.serialization.types.element;
namespace ecologylabFundamentalTestCases.Maps
{
    class SimplData : ElementState, Mappable
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
