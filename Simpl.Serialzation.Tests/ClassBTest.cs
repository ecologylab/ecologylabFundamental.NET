using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Graph;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class ClassBTest
    {
        [TestMethod]
        public void ClassBXml()
        {
            SimplTypesScope.EnableGraphSerialization();

            ClassB test = new ClassB(1, 2);
            ClassA classA = new ClassA(3, 4, test);

            test.ClassA = classA;

            SimplTypesScope tScope = SimplTypesScope.Get("classB", typeof(ClassA), typeof(ClassB));

            TestMethods.TestSimplObject(test, tScope, Format.Xml);

            SimplTypesScope.DisableGraphSerialization();
        }

        //        [TestMethod]
        public void ClassBJson()
        {
            SimplTypesScope.EnableGraphSerialization();

            ClassA test = new ClassA(1, 2);
            ClassB classB = new ClassB(3, 4, test);

            test.ClassB = classB;

            SimplTypesScope tScope = SimplTypesScope.Get("classA", typeof(ClassA), typeof(ClassB));

            TestMethods.TestSimplObject(test, tScope, Format.Json);

            SimplTypesScope.DisableGraphSerialization();
        }
    }
}
