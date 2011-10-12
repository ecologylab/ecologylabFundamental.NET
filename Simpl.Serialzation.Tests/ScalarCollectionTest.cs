using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Scalar;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class ScalarCollectionTest
    {
        [TestMethod]
        public void ScalarCollectionXml()
        {
            ScalarCollection sc = new ScalarCollection();
            
            sc.AddInt(2);
            sc.AddInt(3);
            sc.AddInt(4);
            sc.AddInt(5);
            sc.AddInt(6);
            sc.AddInt(7);

            SimplTypesScope scalarCollectionSimplTypesScope = SimplTypesScope.Get(
                "scalarCollectionTScope", typeof (ScalarCollection));

            TestMethods.TestSimplObject(sc, scalarCollectionSimplTypesScope, StringFormat.Xml);

        }

        [TestMethod]
        public void ScalarCollectionJson()
        {
            ScalarCollection sc = new ScalarCollection();

            sc.AddInt(2);
            sc.AddInt(3);
            sc.AddInt(4);
            sc.AddInt(5);
            sc.AddInt(6);
            sc.AddInt(7);

            SimplTypesScope scalarCollectionSimplTypesScope = SimplTypesScope.Get(
                "scalarCollectionTScope", typeof(ScalarCollection));

            TestMethods.TestSimplObject(sc, scalarCollectionSimplTypesScope, StringFormat.Json);
        }
    }
}
