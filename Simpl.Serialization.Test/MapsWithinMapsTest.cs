using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Maps;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class MapsWithinMapsTest
    {

        [TestMethod]
        public void MapsWithinMapsTestXml()
        {
            TranslationS test = MapsWithinMaps.CreateObject();
            SimplTypesScope tScope = SimplTypesScope.Get("testScope", typeof (TranslationS),
                                                         typeof (ClassDes),
                                                         typeof (FieldDes));
            TestMethods.TestSimplObject(test, tScope, Format.Xml);
        }

        [TestMethod]
        public void MapsWithinMapsTestJson()
        {
            TranslationS test = MapsWithinMaps.CreateObject();
            SimplTypesScope tScope = SimplTypesScope.Get("testScope", typeof (TranslationS),
                                                         typeof (ClassDes),
                                                         typeof (FieldDes));
            TestMethods.TestSimplObject(test, tScope, Format.Json);
        }
    }
}