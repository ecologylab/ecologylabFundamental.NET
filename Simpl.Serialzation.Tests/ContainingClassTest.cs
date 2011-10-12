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
        public void ContainingClassXml()
        {
            SimplTypesScope translationScope = SimplTypesScope.Get("test", typeof (ContainingClass),
                                                                   typeof (ChildClass1), typeof (ChildClass2),
                                                                   typeof (BaseClass));

            ContainingClass ccb = new ContainingClass {TheField = new BaseClass()};

            TestMethods.TestSimplObject(ccb, translationScope, StringFormat.Xml);

            ContainingClass cc1 = new ContainingClass {TheField = new ChildClass1()};

            TestMethods.TestSimplObject(cc1, translationScope, StringFormat.Xml);

            ContainingClass cc2 = new ContainingClass {TheField = new ChildClass2()};

            TestMethods.TestSimplObject(cc2, translationScope, StringFormat.Xml);
        }

        //        [TestMethod]
        public void ContainingClassJson()
        {
            SimplTypesScope translationScope = SimplTypesScope.Get("test", typeof (ContainingClass),
                                                                   typeof (ChildClass1), typeof (ChildClass2),
                                                                   typeof (BaseClass));

            ContainingClass ccb = new ContainingClass {TheField = new BaseClass()};

            TestMethods.TestSimplObject(ccb, translationScope, StringFormat.Json);

            ContainingClass cc1 = new ContainingClass {TheField = new ChildClass1()};

            TestMethods.TestSimplObject(cc1, translationScope, StringFormat.Json);

            ContainingClass cc2 = new ContainingClass {TheField = new ChildClass2()};

            TestMethods.TestSimplObject(cc2, translationScope, StringFormat.Json);
        }
    }
}