namespace Simpl.Serialzation.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Simpl.Serialization;
    using Simpl.Serialization.Library.Graph;
    using Simpl.Serialization.Attributes;
    using Simpl.Serialzation.Tests.TestHelper;
    using Simpl.Serialization.Library.Maps;



    /// <summary>
    /// A test class with collections and map fields
    /// </summary>
    internal class SimplClassWithCollectionAndMaps
    {
        /// <summary>
        /// A map for testing
        /// </summary>
        [SimplMap("map")]
        public Dictionary<string, int> map;

        /// <summary>
        /// A list for testing
        /// </summary>
        [SimplCollection("list")]
        public List<string> list;

        /// <summary>
        /// Creates an instance of <see cref="SimplClassWithCollectionAndMaps"/> for testing purposs
        /// </summary>
        /// <param name="d"></param>
        /// <param name="l"></param>
        public SimplClassWithCollectionAndMaps(Dictionary<string, int> d, List<string> l)
        {
            this.map = d;
            this.list = l;
        }
    }


    /// <summary>
    /// Tests the TestMethods. So meta!
    /// </summary>
    [TestClass]
    public class TestMethodsUnitTest
    {
        /// <summary>
        /// Creates an instance of <see cref="TestMethodsUnitTest"/>
        /// </summary>
        public TestMethodsUnitTest()
        {
            // We really don't need any per-instance setup code
        }

        /// <summary>
        /// Our test context
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>
        ///Test Context... we won't use this here. 
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        /// <summary>
        /// Two simpl classes that will be serialized and deserialized that have the same internal values should be considered equvilant by our testMethods
        /// </summary>
        [TestMethod]
        public void EquivilantSimplClassesAreConsideredEquivilant()
        {
            ClassA test = new ClassA(1, 2);
            ClassB classB = new ClassB(3, 4, test);
            test.ClassB = classB;


            ClassA anotherA = new ClassA(1, 2);
            ClassB anotherB = new ClassB(3, 4, anotherA);
            anotherA.ClassB = anotherB;

            Assert.IsFalse(anotherA == test, "Should be different objects, Class A");
            Assert.IsFalse(anotherB == classB, "Should be different objects, Class B");

            Assert.IsTrue(!TestMethods.CompareOriginalObjectToDeserializedObject(classB, anotherB).Any(), "ClassB's in both cases should be the same");
            Assert.IsTrue(!TestMethods.CompareOriginalObjectToDeserializedObject(test, anotherA).Any(), "ClassA's should both be the same");
        }

        /// <summary>
        /// Two simpl classes that differ should be considered different
        /// </summary>
        [TestMethod]
        public void DifferentSimplClassesAreConsideredDifferent()
        {
            ClassA test = new ClassA(1, 2);
            ClassB classB = new ClassB(3, 3, test);
            test.ClassB = classB;

            ClassA anotherA = new ClassA(1, 2);
            ClassB anotherB = new ClassB(3, 4, anotherA);
            anotherA.ClassB = anotherB;

            Assert.IsFalse(anotherA == test, "Should be different objects, Class A");
            Assert.IsFalse(anotherB == classB, "Should be different objects, Class B");

            Assert.IsTrue(TestMethods.CompareOriginalObjectToDeserializedObject(classB, anotherB).Any(), "ClassB's in both cases should not be the same");
            Assert.IsTrue(TestMethods.CompareOriginalObjectToDeserializedObject(test, anotherA).Any(), "ClassA's should not both be the same");
        }


        /// <summary>
        /// Creates a basic instance of a class with lists and maps
        /// </summary>
        /// <returns>A fresh instance. TWo calls to createInstance() should create two distinct entities that are equal</returns>
        private SimplClassWithCollectionAndMaps createInstance()
        {
            var l = new List<string> { "a", "b", "c" };
            var d = new Dictionary<string, int>();
            d.Add("a", 1);
            d.Add("b", 2);
            d.Add("c", 3);

            return new SimplClassWithCollectionAndMaps(d, l);
        }

        /// <summary>
        /// Lists and Maps should have certain type descriptions information.
        /// (This test added to root out a bug)
        /// </summary>
        [TestMethod]
        public void TypeDescriptorEmittedCorrectlyForMapAndListFields()
        {
            var collectionAndMaps = createInstance();

            var typeDescriptor = ClassDescriptor.GetClassDescriptor(collectionAndMaps.GetType());

            var listDescriptor = typeDescriptor.FieldDescriptorByFieldName["list"];
            Assert.IsTrue(listDescriptor.IsCollection, "A list is a collection");

            var mapDescriptor = typeDescriptor.FieldDescriptorByFieldName["map"];
            Assert.IsTrue(listDescriptor.IsCollection, "A map is also a collection.");


        }

        /// <summary>
        /// Comparison between two lists that are similar or disimilar should work.
        /// </summary>
        [TestMethod]
        public void ListComparisonWorks()
        {
            var firstList = Enumerable.Range(0, 10).ToList();
            var sameList = Enumerable.Range(0, 10).ToList();

            Assert.IsTrue(TestMethods.AreSimplListsEqual(firstList,sameList));
            
            var differentList = Enumerable.Range(20,10).ToList();
            Assert.IsFalse(TestMethods.AreSimplListsEqual(firstList,differentList));

            differentList.RemoveAll(everything => true);

            Assert.IsFalse(TestMethods.AreSimplListsEqual(firstList, differentList));            

        }

        /// <summary>
        /// Comparisons between two dictionaries that are the same or different should work correctly
        /// </summary>
        [TestMethod]
        public void DictionaryComparisonWorks()
        {
            // Handle the base case. :D 
            var stringIntDictionary = createInstance().map;
            var sameStringIntDictionary = createInstance().map;

            var differentStringIntDictionary = createInstance().map;
            differentStringIntDictionary.Add("difference", 13);

            Assert.IsTrue(TestMethods.AreSimplDictionariesEqual(stringIntDictionary,sameStringIntDictionary));
            Assert.IsFalse(TestMethods.AreSimplDictionariesEqual(stringIntDictionary,differentStringIntDictionary));

            // Try again with an empty dictionary

            differentStringIntDictionary.Clear();
            Assert.IsFalse(TestMethods.AreSimplDictionariesEqual(stringIntDictionary, differentStringIntDictionary));
        }

        /// <summary>
        /// Simpl.Classes with equivilant internal lists and dictionaries should be treated correctly
        /// </summary>
        [TestMethod]
        public void EquivilantListsAndMapsAreTreatedCorreclty()
        {
            var classA = createInstance();
            var classB = createInstance();


            Assert.IsTrue(ClassDescriptor.GetClassDescriptor(classA.GetType()).AllFieldDescriptors.Count() == 2, "Incorrect number of field descriptors");
            Assert.IsFalse(classA == classB, "Should be different instances");
            Assert.IsFalse(TestMethods.CompareOriginalObjectToDeserializedObject(classA, classB).Any(), "Class A and B should be the same... their collections are equivilant.");
        }


        /// <summary>
        /// Two classes that have non-equal lists or maps should not be considered the same
        /// </summary>
        [TestMethod]
        public void NonEquivilantListsAndMapsAreTreatedCorreclty()
        {
            var baseInstance = createInstance();
            var differentListInstance = createInstance();
            differentListInstance.list.Add("difference");

            var differentMapInstance = createInstance();
            differentMapInstance.map.Add("differnce", 13);


            Assert.IsFalse(baseInstance == differentListInstance, "A, B Should be different instances");
            Assert.IsFalse(baseInstance == differentMapInstance, "A and C should be different instances");
            Assert.IsFalse(TestMethods.CompareOriginalObjectToDeserializedObject(baseInstance, differentListInstance).Any(), "Class A and B should not be the same... B's list is different.");
            Assert.IsTrue(TestMethods.CompareOriginalObjectToDeserializedObject(baseInstance, differentMapInstance).Any(), "Class A and C should not be the same... C's dictionary is different.");
        }

        /// <summary>
        /// Checks that the MapsWithinMaps compare correctly
        /// </summary>
        [TestMethod]
        public void MapsWithSimplValuesCompareCorrectly()
        {
            var instanceOne = MapsWithinMaps.CreateObject();
            var instanceTwo = MapsWithinMaps.CreateObject();

            var instanceOneDictionary = instanceOne.entriesByTag;
            var instanceTwoDictionary = instanceTwo.entriesByTag;

            TestMethods.AssertSimplTypesEqual(instanceOneDictionary, instanceTwoDictionary, "Dictionaries should be the same");
            TestMethods.AssertSimplTypesEqual(instanceOne, instanceTwo, "Instances should be the same.");

        }
    }
}
