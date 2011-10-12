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
    public class ClassATest
    {
        [TestMethod]
        public void ClassAXml()
        {
            SimplTypesScope.EnableGraphSerialization();

            ClassA test = new ClassA(1, 2);
            ClassB classB = new ClassB(3, 4, test);

            test.ClassB = classB;

            SimplTypesScope tScope = SimplTypesScope.Get("classA", typeof (ClassA), typeof (ClassB));

            TestMethods.TestSimplObject(test, tScope, StringFormat.Xml);

            SimplTypesScope.DisableGraphSerialization();
        }

        [TestMethod]
        public void ClassAJson()
        {
            SimplTypesScope.EnableGraphSerialization();

            ClassA test = new ClassA(1, 2);
            ClassB classB = new ClassB(3, 4, test);

            test.ClassB = classB;

            SimplTypesScope tScope = SimplTypesScope.Get("classA", typeof(ClassA), typeof(ClassB));

            TestMethods.TestSimplObject(test, tScope, StringFormat.Json);

            SimplTypesScope.DisableGraphSerialization();
        }
    }
}
