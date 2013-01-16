using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Inheritence;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class ContainingClassTest
    {
        [TestMethod]
        public void ContainingClassCcbXml()
        {
            SimplTypesScope translationScope = SimplTypesScope.Get("test", typeof (ContainingClass),
                                                                   typeof (ChildClass1), typeof (ChildClass2),
                                                                   typeof (BaseClass));

            ContainingClass ccb = new ContainingClass {TheField = new BaseClass()};

            TestMethods.TestSimplObject(ccb, translationScope, Format.Xml);
        }

        [TestMethod]
        public void ContainingClassCc1Xml()
        {
            SimplTypesScope translationScope = SimplTypesScope.Get("test", typeof(ContainingClass),
                                                                   typeof(ChildClass1), typeof(ChildClass2),
                                                                   typeof(BaseClass));
            ContainingClass cc1 = new ContainingClass { TheField = new ChildClass1() };

            TestMethods.TestSimplObject(cc1, translationScope, Format.Xml);
        }

        [TestMethod]
        public void ContainingClassCc2Xml()
        {
            SimplTypesScope translationScope = SimplTypesScope.Get("test", typeof(ContainingClass),
                                                                   typeof(ChildClass1), typeof(ChildClass2),
                                                                   typeof(BaseClass));

            ContainingClass cc2 = new ContainingClass { TheField = new ChildClass2() };

            TestMethods.TestSimplObject(cc2, translationScope, Format.Xml);
        }

        [TestMethod]
        public void ContainingClassCcbJson()
        {
            SimplTypesScope translationScope = SimplTypesScope.Get("test", typeof(ContainingClass),
                                                                   typeof(ChildClass1), typeof(ChildClass2),
                                                                   typeof(BaseClass));

            ContainingClass ccb = new ContainingClass { TheField = new BaseClass() };

            TestMethods.TestSimplObject(ccb, translationScope, Format.Json);
        }

        [TestMethod]
        public void ContainingClassCc1Json()
        {
            SimplTypesScope translationScope = SimplTypesScope.Get("test", typeof(ContainingClass),
                                                                   typeof(ChildClass1), typeof(ChildClass2),
                                                                   typeof(BaseClass));
            ContainingClass cc1 = new ContainingClass { TheField = new ChildClass1() };

            TestMethods.TestSimplObject(cc1, translationScope, Format.Json);
        }

        [TestMethod]
        public void ContainingClassCc2Json()
        {
            SimplTypesScope translationScope = SimplTypesScope.Get("test", typeof(ContainingClass),
                                                                   typeof(ChildClass1), typeof(ChildClass2),
                                                                   typeof(BaseClass));

            ContainingClass cc2 = new ContainingClass { TheField = new ChildClass2() };

            TestMethods.TestSimplObject(cc2, translationScope, Format.Json);
        }
    }
}