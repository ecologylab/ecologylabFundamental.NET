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
            TranslationScope circleTransaltionScope = TranslationScope.Get("circleTScope", typeof(Circle),
                                                                          typeof(Point));
            TestMethods.TestSimplObject(c, circleTransaltionScope, StringFormat.Xml);
        }

        [TestMethod]
        public void CircleJson()
        {
            Circle c = new Circle(new Point(1, 3), 3);
            TranslationScope circleTransaltionScope = TranslationScope.Get("circleTScope", typeof(Circle),
                                                                          typeof(Point));
            TestMethods.TestSimplObject(c, circleTransaltionScope, StringFormat.Json);
        }
    }
}
