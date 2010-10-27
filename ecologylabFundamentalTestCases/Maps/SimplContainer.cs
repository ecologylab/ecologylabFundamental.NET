using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.attributes;
using ecologylabFundamental.ecologylab.serialization;

namespace ecologylabFundamentalTestCases.Maps
{

    class SimplContainer : ElementState
    {

        [simpl_map("items")]
        [simpl_nowrap]
        public Dictionary<String, SimplData> dictSimplData = new Dictionary<String, SimplData>();

        public void fillDictionary()
        {
            SimplData data1 = new SimplData();
            data1.itemKey = "1";
            data1.testData = "data1";

            dictSimplData.Add(data1.itemKey, data1);


            SimplData data2 = new SimplData();
            data2.itemKey = "2";
            data2.testData = "data2";

            dictSimplData.Add(data2.itemKey, data2);

            SimplData data3 = new SimplData();
            data3.itemKey = "3";
            data3.testData = "data3";

            dictSimplData.Add(data3.itemKey, data3);

            SimplData data4 = new SimplData();
            data4.itemKey = "4";
            data4.testData = "data4";

            dictSimplData.Add(data4.itemKey, data4);

        }
    }
}
