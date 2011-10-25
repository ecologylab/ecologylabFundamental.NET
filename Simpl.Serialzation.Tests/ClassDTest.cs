using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Graph.Diamond;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class ClassDTest
    {
        [TestMethod]
        public void ClassDXml()
        {
            SimplTypesScope.EnableGraphSerialization();

            ClassC classC = new ClassC();
            ClassD test = new ClassD(new ClassA(classC), new ClassB(classC));

            SimplTypesScope tScope = SimplTypesScope.Get("classD", typeof (ClassA), typeof (ClassB),
                                                         typeof (ClassC), typeof (ClassD), typeof (ClassX));

            TestMethods.TestSimplObject(test, tScope, Format.Xml);

            SimplTypesScope.DisableGraphSerialization();
        }

//        [TestMethod]
        public void ClassDJson()
        {
            SimplTypesScope.EnableGraphSerialization();

            ClassC classC = new ClassC();
            ClassD test = new ClassD(new ClassA(classC), new ClassB(classC));

            SimplTypesScope tScope = SimplTypesScope.Get("classD", typeof(ClassA), typeof(ClassB),
                                                         typeof(ClassC), typeof(ClassD), typeof(ClassX));

            TestMethods.TestSimplObject(test, tScope, Format.Json);

            SimplTypesScope.DisableGraphSerialization();
        }
    }
}
