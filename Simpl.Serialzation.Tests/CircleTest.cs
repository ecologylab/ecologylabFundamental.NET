using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class CircleTest
    {
        [TestMethod]
        public void CircleSerialization()
        {
            Circle c = new Circle(new Point(1,3), 3);
            ClassDescriptor.Serialize(c, StringFormat.Xml, Console.Out);
        }
    }
}
