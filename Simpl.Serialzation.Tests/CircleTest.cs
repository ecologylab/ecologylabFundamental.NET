using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Circle;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class CircleTest
    {
        [TestMethod]
        public void CircleXml()
        {
            Circle c = new Circle(new Point(1, 3), 3);
            SimplTypesScope circleTransaltionScope = SimplTypesScope.Get("circleTScope", typeof(Circle),
                                                                          typeof(Point));
            TestMethods.TestSimplObject(c, circleTransaltionScope, Format.Xml);
        }

        [TestMethod]
        public void CircleJson()
        {
            Circle c = new Circle(new Point(1, 3), 3);
            SimplTypesScope circleTransaltionScope = SimplTypesScope.Get("circleTScope", typeof(Circle),
                                                                          typeof(Point));
            TestMethods.TestSimplObject(c, circleTransaltionScope, Format.Json);
        }

//        [TestMethod]
        public void CircleTlv()
        {
            Circle c = new Circle(new Point(1, 3), 3);
            SimplTypesScope circleTransaltionScope = SimplTypesScope.Get("circleTScope", typeof(Circle),
                                                                          typeof(Point));
            TestMethods.TestSimplObject(c, circleTransaltionScope, Format.Tlv);
        }
    }
}
