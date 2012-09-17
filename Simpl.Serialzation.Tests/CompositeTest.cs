namespace Simpl.Serialzation.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Simpl.Serialization;
    using Simpl.Serialization.Library.Composite;
    using Simpl.Serialzation.Tests.TestHelper;
    
    [TestClass]
    public class CompositeTest
    {
        [TestMethod]
        public void CompositeClassEmitsCorrectClassDescriptor()
        {
            var containerDescriptor = ClassDescriptor.GetClassDescriptor(typeof(Container));
            var wcBaseDescriptor = ClassDescriptor.GetClassDescriptor(typeof(WcBase));

            var wcDesc = containerDescriptor.GetFieldDescriptorByFieldName("wc");

            Assert.IsTrue(wcDesc != null, "No field descriptor for wc!");
            Assert.AreEqual(typeof(WcBase),wcDesc.Field.FieldType);
            Assert.IsTrue(ClassDescriptor.GetClassDescriptor(wcDesc.Field.FieldType).Equals(wcBaseDescriptor),string.Format("wc Field type is not correct! {0} | {1}",wcDesc,ClassDescriptor.GetClassDescriptor(wcDesc.Field.MemberType)));

            var instance = new Container(new WcBase(1));         
        }
            
        public class InternalTestClass
        {
            [Simpl.Serialization.Attributes.SimplScalar]
            public int ourScalar = 5;

            public InternalTestClass()
            {
            }

            public InternalTestClass(int A)
            {
                this.ourScalar = A;
            }
        }

        [TestMethod]
        public void SimplScalarTagReprIsFieldName()
        {
            var testObj = new InternalTestClass();

            testObj.ourScalar = 3;

            SimplTypesScope sTs = SimplTypesScope.Get("scalarTestScope", typeof(InternalTestClass));

            TestMethods.TestSimplObject(testObj, sTs);

            var resultStream = TestMethods.TestSerialization(testObj, Format.Xml);

            string xmlString = resultStream.StringData;

            XElement xe = XElement.Parse(xmlString);

            Assert.IsTrue(xe.Attributes("our_scalar").Any(), "Should have an XML scalar attribute \"our_scalar\"");
            Assert.IsTrue(int.Parse(xe.Attribute("our_scalar").Value.ToString()) == 3, "Our scalar value should be equal to 3!");
        }


        public class InternalCompositeTestClass
        {
            [Simpl.Serialization.Attributes.SimplComposite]
            public InternalTestClass internalComposite;

            public InternalCompositeTestClass()
            {
            }

            public InternalCompositeTestClass(int A)
            {
                this.internalComposite = new InternalTestClass(A);
            }
        }

        [TestMethod]
        public void SimplCompositeTagReprIsFieldName()
        {
            var testObj = new InternalCompositeTestClass(5);

            SimplTypesScope sTs = SimplTypesScope.Get("compTestScope", typeof(InternalCompositeTestClass), typeof(InternalTestClass));

            TestMethods.TestSimplObject(testObj, sTs);

            var resultStream = TestMethods.TestSerialization(testObj, Format.Xml);

            string xmlString = resultStream.StringData;

            XElement xe = XElement.Parse(xmlString);

            Assert.IsTrue(xe.Elements("internal_composite").Any(), "Should have an XML composite element \"internal_composite\"");
        }


        public class InternalPolymorphicCompositeTestClass
        {
            [Simpl.Serialization.Attributes.SimplClasses(new []{typeof(InternalTestClass), typeof(InternalCompositeTestClass)})]
            [Simpl.Serialization.Attributes.SimplComposite]
            public object polyMorph;

            public InternalPolymorphicCompositeTestClass()
            {
            }

            public InternalPolymorphicCompositeTestClass(int A, bool itc)
            {
                polyMorph = itc ? (object)new InternalTestClass(A) : (object)new InternalCompositeTestClass(A);
            }
        }

        [TestMethod]
        public void SimplPolymorphicCompositeFieldDescriptorFollowsTagNameConventions()
        {
            // The case below double covers this slightly, but this makes the issue a bit easier to uncover.


            var v = new InternalPolymorphicCompositeTestClass(3,true);

            ClassDescriptor cd = ClassDescriptor.GetClassDescriptor(v);

            var polymorphicField = cd.FieldDescriptorByFieldName["polyMorph"];

            Assert.AreEqual("polyMorph", polymorphicField.Name, "Name incorrect!");
            Assert.AreEqual("poly_morph",polymorphicField.TagName,"Tag name incorrect!");

            Assert.IsTrue(polymorphicField.IsPolymorphic, "Should be polymorphic!");


            var psuedoDescriptor = ClassDescriptor.GetClassDescriptor(v.polyMorph).PseudoFieldDescriptor;

            Assert.AreEqual("polyMorph", psuedoDescriptor.Name, "Psuedo Descriptor is wrecking life.");
            Assert.AreEqual("poly_morph", psuedoDescriptor.TagName, "Psuedo descriptor is again messing stuff up.");

        }

        [TestMethod]
        public void SimplPolymorphicCompositeFollowsTagNameConventions()
        {
            // case A: We have an ITC in the polymorphic class

            // Create the object
            var ourObjCaseA = new InternalPolymorphicCompositeTestClass(4, true);

            // Make sure that we created it correctly
            Assert.IsTrue(ourObjCaseA.polyMorph.GetType().Equals(typeof(InternalTestClass)), "Type should be internalTestClass");

            // Create the sts
            var ourSTS = new SimplTypesScope("CompositeWithPolymorph", new[] { typeof(InternalTestClass), typeof(InternalCompositeTestClass), typeof(InternalPolymorphicCompositeTestClass) });
            
            // Serialize it, get the XML representation, parse it
            var resultStream = TestMethods.TestSerialization(ourObjCaseA, Format.Xml);
            string xmlString = resultStream.StringData;
            XElement xe = XElement.Parse(xmlString);

            // polyMorph => poly_morph. And we should expect that element in our serialized representation. 
            Assert.IsTrue(xe.Elements("poly_morph").Any(), "Should have an XML composite element \"poly_morph\"");

            // Okay, let's do case B, an Internal Composite inside. ;D 
            var ourObjCaseB = new InternalPolymorphicCompositeTestClass(5, false);

            // Yes, I know this could probably be a seperate method. Just putting it here beacuse it fit in the moment. 
            // Feel free to refactor if it tickles your fancy as such.

            // Serialize it, get the XML representation, parse it
            var resultStreamCaseB = TestMethods.TestSerialization(ourObjCaseB, Format.Xml);
            string xmlStringCaseB =  resultStreamCaseB.StringData;
            XElement xeCaseB = XElement.Parse(xmlStringCaseB);

            // polyMorph => poly_morph. And we should expect that element in our serialized representation. 
            Assert.IsTrue(xeCaseB.Elements("poly_morph").Any(), "Should have an XML composite element \"poly_morph\"");
            
            // We'll defer these tests to the end; if we have a failure earlier we'll end up getting a deserialized null 
            // because we fail hard if we don't serialize the type name correctly. 
            TestMethods.TestSimplObject(ourObjCaseA, ourSTS);
            TestMethods.TestSimplObject(ourObjCaseB, ourSTS);
        }

        
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

            TestMethods.TestSimplObject(c, simplTypesScope, Format.Json);
        }

        [TestMethod]
        public void CompositeSubOneJson()
        {
            Container c = new Container(new WcSubOne("testing", 1));
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("compositeTScope", typeof(Container),
                                                                     typeof(WcBase), typeof(WcSubOne),
                                                                     typeof(WcSubTwo));

            TestMethods.TestSimplObject(c, simplTypesScope, Format.Json);
        }

        [TestMethod]
        public void CompositeSubTwoJson()
        {
            Container c = new Container(new WcSubTwo(true, 1));
            SimplTypesScope simplTypesScope = SimplTypesScope.Get("compositeTScope", typeof(Container),
                                                                     typeof(WcBase), typeof(WcSubOne),
                                                                     typeof(WcSubTwo));

            TestMethods.TestSimplObject(c, simplTypesScope, Format.Json);
        }
    }
}
