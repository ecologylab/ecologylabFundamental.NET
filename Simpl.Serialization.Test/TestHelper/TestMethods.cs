namespace Simpl.Serialzation.Tests.TestHelper
{
    using System;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Simpl.Serialization;
    using Simpl.Serialization.Attributes;
    using Simpl.Serialization.Types;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Helper function to serialize and deserialize objects 
    /// </summary>
    public static class TestMethods
    {
        /// <summary>
        /// serializes data and returns an the serialized data as a stream for application to use. 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static HelperStream TestSerialization(Object obj, Format format)
        {
            HelperStream hStream = new HelperStream();
            SimplTypesScope.Serialize(obj, hStream, format);
            /*switch (format)
            {
                case Format.Tlv:
                    PrettyPrint.PrintBinary(hStream.BinaryData, format);
                    break;
                default:
                    PrettyPrint.PrintString(hStream.StringData, format);
                    break;
            }*/
            Console.WriteLine(Encoding.UTF8.GetString(hStream.ToArray()));
            return new HelperStream(hStream.BinaryData);
        }

        /// <summary>
        /// deseiralizes the data, given the input stream and format. returns object representation of the input data. 
        /// </summary>
        /// <param name="simplTypesScope">Simpl type scope for deserialization</param>
        /// <param name="inputStream">Input stream of the serialized representaiton</param>
        /// <param name="format">Format of the serialization</param>
        /// <returns>a deserialized object from the type scope</returns>
        public static Object TestDeserialization(SimplTypesScope simplTypesScope, Stream inputStream, Format format)
        {
            Object deserializedObj = simplTypesScope.Deserialize(inputStream, format);
            return deserializedObj;
        }

        /// <summary>
        /// Helper methods to test de/serialization of and input object and simpl type scope in particular format
        /// </summary>
        /// <param name="originalObject">Original object</param>
        /// <param name="simplTypesScope">Type scope to attempt serialization and deserializaton</param>
        /// <param name="format">Format to de/serialize in</param>
        public static void TestSimplObject(Object originalObject, SimplTypesScope simplTypesScope, Format format)
        {
            Console.WriteLine("Serializing object " + originalObject);
            Console.WriteLine("-----------------------------------------------------------------------------");
            HelperStream originalStream = TestSerialization(originalObject, format);

            Console.WriteLine();
            Object deserializedObj = TestDeserialization(simplTypesScope, originalStream, format);
    
            TestMethods.AssertSimplTypesEqual(originalObject, deserializedObj);

            Console.WriteLine("Deserialized object " + deserializedObj);
            Console.WriteLine("-----------------------------------------------------------------------------");
            HelperStream deserializedReserializedStream = TestSerialization(deserializedObj, format);
        }

        /// <summary>
        /// simplified overload method to test de/serialization in Xml only.
        /// </summary>
        /// <param name="obj">Object to de/serialize</param>
        /// <param name="simplTypesScope">Type scope containing the object</param>
        public static void TestSimplObject(Object obj, SimplTypesScope simplTypesScope)
        {
            TestSimplObject(obj, simplTypesScope, Format.Xml);
        }

        /// <summary>
        /// Compares two objects, one the original representation of the object, and another from the roundtrip serialization and deserialization.
        /// Does a comparison of the [Simpl...] members of the two objects, comparing values. 
        /// <para>If the objects are comparable, makes sure that compare() returns 0
        /// If the objects are equatable, makes sure that Equals(other) returns true
        /// </para>
        /// </summary>
        /// <param name="originalObject">Original object to test</param>
        /// <param name="deserializedObject">Deserialized representation</param>
        /// <returns>True if the objects are equal, false if they are not. True implies that roundtripping the original object worked as expected</returns>
        public static List<SimplIssue> CompareOriginalObjectToDeserializedObject(object originalObject, object deserializedObject)
        {
            var orig = new List<object>();
            var deser = new List<object>();
            var issues = new List<SimplIssue>();

            CompareObjectsRecursive(originalObject, deserializedObject, orig, deser, issues);

            return issues;
        }





        /// <summary>
        /// Determines iff one of the two items is null. Implies that the two are not equal and not both null
        /// </summary>
        /// <param name="originalObject">LHS of this operation</param>
        /// <param name="deserializedObject">RHS of this operation</param>
        /// <returns>True iff one is null</returns>
        private static bool OneIsNullAndOtherIsNot(object originalObject, object deserializedObject)
        {
            return ExactlyOneIsNull(originalObject, deserializedObject);
        }

        /// <summary>
        /// Determines if exactly one object in the collection is null
        /// </summary>
        /// <param name="objects">All objects</param>
        /// <returns>True if exactly one is null</returns>
        private static bool ExactlyOneIsNull(params object[] objects)
        {
            // Exactly one equal to null. 
            return objects.Where(obj => obj == null).Count() == 1;
        }

        /// <summary>
        /// Recursively compares two objects to ensure that they are the same. 
        /// Compares only values in the objects that will be simpl de/serialized
        /// </summary>
        /// <param name="originalObject">Original object</param>
        /// <param name="deserializedObject">Deserialized representation, the result of round-tripping</param>
        /// <param name="originalCompared">List of compared composite objects, prevents graph infinite loops</param>
        /// <param name="derserializedCompared">List of compared composite objects, prevents graph infinite loops</param>
        /// <param name="issues">List of issues found in the comparison</param>
        /// <returns>True if both are equal, false otherwise</returns>
        private static bool CompareObjectsRecursive(object originalObject, object deserializedObject, List<object> originalCompared, List<object> derserializedCompared, List<SimplIssue> issues)
        {
            if (originalObject == null && deserializedObject == null)
            {
                // both null is questionably acceptable. :|
                System.Diagnostics.Debug.WriteLine("Both null for deserialization... seems questionable.", category: "Comparison warning.");
                return true;
            }
            else
            {
                // They are not equal here, obviously
                if (OneIsNullAndOtherIsNot(originalObject, deserializedObject))
                {
                    issues.Add(new SimplIssue("One object null, the other wasn't.", originalObject, deserializedObject));
                    return false;
                }
            }

            // Add our objects to the compared object lists
            originalCompared.Add(originalObject);
            derserializedCompared.Add(deserializedObject);


            
            // Compare object types... Make sure they are the same.
            if (originalObject.GetType().Equals(deserializedObject.GetType()))
            {
                // Get a class descriptor for both. 

                var classDescriptor = ClassDescriptor.GetClassDescriptor(originalObject.GetType());

                foreach (var fieldDescriptor in classDescriptor.AllFieldDescriptors)
                {
                    var originalDescribedObject = fieldDescriptor.GetValue(originalObject);
                    var deserializedDescribedObject = fieldDescriptor.GetValue(deserializedObject);

                    if (originalCompared.Contains(originalDescribedObject) || derserializedCompared.Contains(deserializedDescribedObject))
                    {
                        // skip if we've already compared these.
                        continue;
                    }

                    if (fieldDescriptor.IsComposite)
                    {
                        // Catches a self-reference
                        if (Object.ReferenceEquals(originalDescribedObject, originalObject) || Object.ReferenceEquals(deserializedDescribedObject, deserializedObject))
                        {
                            // Circular refernces are fine, skip them / move on.
                            continue;
                        }

                        //Recurse and compare the inner objects
                        bool result = CompareObjectsRecursive(originalDescribedObject, deserializedDescribedObject, originalCompared, derserializedCompared, issues);
                        if (!result)
                        {
                            issues.Add(new SimplIssue("The composite objects are not equal.", originalDescribedObject, deserializedDescribedObject));
                            return false;
                        }
                    }
                    else
                    {
                        if (fieldDescriptor.IsCollection)
                        {
                            // We have three options here; either the field is an IDictionary, in which case we need to sort and compare keys
                            // or we have an IList, in which case we should sort and compare values. 
                            // or we have another collection that we don't support for some reason. Throw exception.

                            var collectionType = originalDescribedObject.GetType();

                            var comparator = new SimplObjectEqualityComparer();
                            
                            var result = comparator.Equals(originalDescribedObject, deserializedDescribedObject);
                            
                            if(!result)
                            {
                                issues.Add(new SimplIssue("Collections between two types differ in values", originalDescribedObject, deserializedDescribedObject));
                            }
                            
                            return result;
                        }
                        else
                        {
                            if (fieldDescriptor.IsScalar)
                            {
                                if(!fieldDescriptor.ContextSimplEquals(originalObject, deserializedObject))
                                {
                                    issues.Add(new SimplIssue("RHS and LHS scalars are different", originalDescribedObject.ToString(), deserializedDescribedObject.ToString()));
                                    return false;
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Given field descriptor cannot be compared in the test method yet. Type: {0}", fieldDescriptor.CSharpTypeName);
                            }
                        }
                    }
                }
            }
            else
            {
                issues.Add(new SimplIssue("Types of original object and deserialized object differ.", originalObject.GetType(), deserializedObject.GetType()));
                return false;
            }

            //If we didn't return false by now, the types are equal.
            return true;
        }

        /// <summary>
        /// Array of SimplAttributes for Member operations
        /// </summary>
        private static Type[] simplAttributes = new[] { typeof(SimplScalar), typeof(SimplCollection), typeof(SimplComposite), typeof(SimplCompositeAsScalar) };


        /// <summary>
        /// Checks if a member has SimplAttributes
        /// </summary>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="member">The member to check</param>
        /// <returns>True if simpl attributes are with the member</returns>
        private static bool MemberHasSimplAttributes<T>(T member) where T : MemberInfo
        {
            return member.GetCustomAttributes(true).Where(attrObj => simplAttributes.Contains(attrObj.GetType())).Any();
        }

        /// <summary>
        /// Obtains all Simpl Properties for a given objecttype
        /// </summary>
        /// <param name="objectType">Type</param>
        /// <returns>An enumerable of simpl Properties</returns>
        public static IEnumerable<PropertyInfo> GetSimplProperties(Type objectType)
        {
            BindingFlags bindingParameters = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance;

            return objectType.GetProperties(bindingParameters).Where(prop => MemberHasSimplAttributes(prop));
        }

        public static bool IsComposite(MemberInfo memberType)
        {
            return memberType.GetCustomAttributes(typeof(SimplComposite), true).Any();
        }

        /// <summary>
        /// Obtains all simpl fields for a given object type
        /// </summary>
        /// <param name="objectType">Type</param>
        /// <returns>Simple Field Info</returns>
        public static IEnumerable<FieldInfo> GetSimplFields(Type objectType)
        {
            return objectType.GetFields().Where(field => MemberHasSimplAttributes(field));
        }

        /// <summary>
        /// Obtains all Simpl Values in a given object 
        /// </summary>
        /// <param name="someObject">Some object</param>
        /// <returns>The values for all Simpl Properties and Simpl Fields</returns>
        public static Dictionary<MemberInfo, Object> GetSimplValuesInObject(object someObject)
        {
            Contract.Requires(someObject != null, "Given simpl object is null!");

            var ourDict = new Dictionary<MemberInfo, object>();

            foreach (var propInfo in GetSimplProperties(someObject.GetType()))
            {

                try
                {
                    var value = propInfo.GetValue(someObject, null);
                    ourDict.Add(propInfo, value);
                }
                catch (Exception e)
                {
                    var value = e;
                    // Deubg.Log(e.message);
                    ourDict.Add(propInfo, e);
                }
            }

            foreach (var fieldInfo in GetSimplFields(someObject.GetType()))
            {
                try
                {

                    var value = fieldInfo.GetValue(someObject);
                    ourDict.Add(fieldInfo, value);
                }
                catch (Exception e)
                {
                    var value = e;

                    // Debug.Log(e.Message);

                    ourDict.Add(fieldInfo, value);

                }
            }

            return ourDict;
        }

        /// <summary>
        /// Perform a sort on values of an ICollection by using the HashCode. Allows for comparisons of types that are 
        /// not guarenteed an order, invariant of those types implementing IComparable or preserving order. 
        /// </summary>
        /// <param name="k">Collection type</param>
        /// <returns>An IEnumerable of objects</returns>
        private static IEnumerable<object> sortByHashCode(ICollection k)
        {
            return k.Cast<object>().OrderByDescending(val => val.GetHashCode());
        }

        /// <summary>
        /// Gets the DictionaryEntries (KVP pairs) ordered by hashcode
        /// Allows comparing dictinaries, which have no order guarentees
        /// </summary>
        /// <param name="dictionary">Dictionary to compare</param>
        /// <returns>The ordered entries</returns>
        internal static IEnumerable<DictionaryEntry> getOrderedEntries(IDictionary dictionary)
        {
            foreach (var key in sortByHashCode(dictionary.Keys))
            {
                yield return new DictionaryEntry(key, dictionary[key]);
            }
        }

        /// <summary>
        /// Determines if two simpl dictionaries are equvilant
        /// </summary>
        /// <param name="left">Left dictionary</param>
        /// <param name="right">Right dictionary</param>
        /// <returns>True if equivilant</returns>
        public static bool AreSimplDictionariesEqual(IDictionary left, IDictionary right)
        {
            Contract.Requires(left != null, "Left is null");
            Contract.Requires(right != null, "right is null");

            return Enumerable.SequenceEqual(getOrderedEntries(left), getOrderedEntries(right));
        }

        /// <summary>
        /// Determines if two simpl lists are equivilant
        /// </summary>
        /// <param name="left">Left item</param>
        /// <param name="right">Right item</param>
        /// <returns>True if the lists are correct</returns>
        public static bool AreSimplListsEqual(IList left, IList right)
        {
            Contract.Requires(left != null, "Left is null");
            Contract.Requires(right != null, "Right is null");

            return Enumerable.SequenceEqual(left.Cast<object>(), right.Cast<object>());
        }

        public static void AssertSimplTypesEqual(object left, object right, string message = "Issues with de/serialization! ")
        {
            var comparisonIssues = TestMethods.CompareOriginalObjectToDeserializedObject(left, right);

            if (comparisonIssues.Any())
            {
                Assert.Fail(message + " Errors were: \r\n" + String.Join("\r\n", comparisonIssues.Select(issue => issue.ToString())));
            }
        }

    }

    /// <summary>
    /// Represents an "Issue" found in simpl de/serialization.
    /// </summary>
    public class SimplIssue
    {
        /// <summary>
        /// Creates an instance of <see cref="SimplIssue"/> representing an issue with serialization, givne a message, original value and desieralized value
        /// </summary>
        /// <param name="message">The message representing this particular issue</param>
        /// <param name="original">The original value</param>
        /// <param name="deserialized">the deserialized value</param>
        public SimplIssue(string message, object original, object deserialized)
        {
            this.Message = message;
            this.OriginalValue = original;
            this.DeserializedValue = deserialized;
        }

        /// <summary>
        /// Gets of sets a string representing a message
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a object that represents the "original" value 
        /// </summary>
        public object OriginalValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value represneting the "deserialized" value
        /// </summary>
        public object DeserializedValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a string representation of the Message, OriginalValue.TosTring() and DeserializedValue.ToString()
        /// </summary>
        /// <returns>The string represnetation of the issue</returns>
        public override string ToString()
        {
            return String.Format("{0} OriginalValue: ({1}) DeserializedValue: ({2})", this.Message, NullOrValue(this.OriginalValue), NullOrValue(this.DeserializedValue));
        }

        /// <summary>
        /// Returns a value
        /// </summary>
        /// <param name="obj">The object to print</param>
        /// <returns>ToString() or "NULL"</returns>
        private string NullOrValue(object obj)
        {
            return obj != null ? PrintCollectionsOrObjects(obj) : "NULL";
        }

        /// <summary>
        /// Prints an object that could be an object, a collection or a map.
        /// </summary>
        /// <param name="obj">Object to print</param>
        /// <returns>string rep of the object. </returns>
        private string PrintCollectionsOrObjects(object obj)
        {
            if (obj is IDictionary)
            {
                return PrintDictionary(obj as IDictionary);
            }

            if (obj is IList)
            {
                return PrintList(obj as IList);
            }

            var simplValues = TestMethods.GetSimplValuesInObject(obj);

            if (simplValues.Any())
            {
                var sb = new StringBuilder();

                foreach (var kvp in simplValues)
                {
                    var valueString = kvp.Value.ToString();
                    if(kvp.Value is IDictionary)
                    {
                        valueString = PrintDictionary(kvp.Value as IDictionary);
                    }
                    if(kvp.Value is IList)
                    {
                        valueString = PrintList(kvp.Value as IList);
                    }

                    sb.AppendFormat("{0} = {1}\r\n",kvp.Key.Name, kvp.Value.ToString());
                }
                return sb.ToString();

            }
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// Prints a dictionary to a string
        /// </summary>
        /// <param name="dict">Dictionary to print</param>
        /// <returns>A string representation of the dictionary</returns>
        private string PrintDictionary(IDictionary dict)
        {
            StringBuilder sb = new StringBuilder("[");

            foreach (DictionaryEntry de in TestMethods.getOrderedEntries(dict))
            {
                // Preferred syntax: [ {key, value}, {key, value} ] 

                sb.Append(String.Format("{{ {0} , ", de.Key.ToString()));
                sb.Append(PrintCollectionsOrObjects(de.Value));
                sb.Append("}}, ");
            }

            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Prints a given Ilist to a string
        /// </summary>
        /// <param name="list">The IList to print</param>
        /// <returns>A string rep</returns>
        private string PrintList(IList list)
        {
            // Preferred syntax: [ {value} , {value} ]
            return "[ " + String.Join(",", list.Cast<object>().Select(obj => PrintCollectionsOrObjects(obj))) + " ]";
        }    
    }
}
