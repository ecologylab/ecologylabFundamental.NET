using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Composite;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class CompositeTest
    {
        [TestMethod]
        public void CompositeBaseXml()
        {
            Container c = new Container(new WcBase(1));
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("compositeTScope", typeof (Container),
                                                                     typeof (WcBase), typeof (WcSubOne),
                                                                     typeof (WcSubTwo));

            TestMethods.TestSimplObject(c, simplTypesScope);
        }

        [TestMethod]
        public void CompositeSubOneXml()
        {
            Container c = new Container(new WcSubOne("testing", 1));
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("compositeTScope", typeof(Container),
                                                                     typeof(WcBase), typeof(WcSubOne),
                                                                     typeof(WcSubTwo));

            TestMethods.TestSimplObject(c, simplTypesScope);
        }

        [TestMethod]
        public void CompositeSubTwoXml()
        {
            Container c = new Container(new WcSubTwo(true, 1));
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("compositeTScope", typeof(Container),
                                                                     typeof(WcBase), typeof(WcSubOne),
                                                                     typeof(WcSubTwo));

            TestMethods.TestSimplObject(c, simplTypesScope);
        }


        [TestMethod]
        public void CompositeBaseJson()
        {
            Container c = new Container(new WcBase(1));
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("compositeTScope", typeof(Container),
                                                                     typeof(WcBase), typeof(WcSubOne),
                                                                     typeof(WcSubTwo));

            TestMethods.TestSimplObject(c, simplTypesScope, StringFormat.Json);
        }

        [TestMethod]
        public void CompositeSubOneJson()
        {
            Container c = new Container(new WcSubOne("testing", 1));
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("compositeTScope", typeof(Container),
                                                                     typeof(WcBase), typeof(WcSubOne),
                                                                     typeof(WcSubTwo));

            TestMethods.TestSimplObject(c, simplTypesScope, StringFormat.Json);
        }

        [TestMethod]
        public void CompositeSubTwoJson()
        {
            Container c = new Container(new WcSubTwo(true, 1));
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("compositeTScope", typeof(Container),
                                                                     typeof(WcBase), typeof(WcSubOne),
                                                                     typeof(WcSubTwo));

            TestMethods.TestSimplObject(c, simplTypesScope, StringFormat.Json);
        }
    }
}
