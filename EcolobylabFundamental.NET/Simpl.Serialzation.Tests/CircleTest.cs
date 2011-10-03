using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library;
using Simpl.Serialization.Library.Circle;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class CircleTest
    {
        [TestMethod]
        public void CircleSerialization()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            Circle c = new Circle(new Point(1, 3), 3);
            ClassDescriptor.Serialize(c, StringFormat.Xml, sw);

            Console.WriteLine(sb);


            TranslationScope circleTransaltionScope = TranslationScope.Get("circleTScope", typeof (Circle),
                                                                           typeof (Point));
            Circle deserializedObj = (Circle) circleTransaltionScope.Deserialize(sb.ToString(), StringFormat.Xml);

            if (deserializedObj != null)
                ClassDescriptor.Serialize(deserializedObj, StringFormat.Xml, Console.Out);
        }
    }
}
