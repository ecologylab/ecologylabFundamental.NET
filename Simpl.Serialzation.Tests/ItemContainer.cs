using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Items;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class ItemContainer
    {
//        [TestMethod]
        public void ItemContainerXml()
        {
            Container c = new Container();
            c.PopulateContainer();

            SimplTypesScope containerTranslationScope = SimplTypesScope.Get("containerScope",
                                                                            typeof (Container), typeof (ItemBase),
                                                                            typeof (ItemOne), typeof (ItemTwo),
                                                                            typeof (ItemRandom));
             
            TestMethods.TestSimplObject(c, containerTranslationScope, Format.Xml);
        }
    }
}