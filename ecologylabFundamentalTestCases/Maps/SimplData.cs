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
        [simpl_scalar]
        public String itemKey;

        [simpl_scalar]
        public String testData;

        public object key()
        {
            return itemKey;
        }
    }
}
