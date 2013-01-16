using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Graph.Collection;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class ContainerTest
    {
        [TestMethod]
        public void ContainerXml()
        {

            SimplTypesScope.EnableGraphSerialization();

            Container test = new Container().InitializeInstance();
            SimplTypesScope translationScope = SimplTypesScope.Get("testcollection", typeof (Container),
                                                                   typeof (ClassA));
            TestMethods.TestSimplObject(test, translationScope, Format.Xml);


            SimplTypesScope.DisableGraphSerialization();
        }

//        [TestMethod]
        public void ContainerJson()
        {

            SimplTypesScope.EnableGraphSerialization();

            Container test = new Container().InitializeInstance();
            SimplTypesScope translationScope = SimplTypesScope.Get("testcollection", typeof(Container),
                                                                   typeof(ClassA));
            TestMethods.TestSimplObject(test, translationScope, Format.Json);


            SimplTypesScope.DisableGraphSerialization();
        }
    }
}
