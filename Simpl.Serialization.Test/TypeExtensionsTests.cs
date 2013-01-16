namespace Simpl.Serialzation.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Simpl.Serialization;
    using System.Linq;
    using System.Collections.Generic;
    using Simpl.Serialization.Attributes;
    
        /// <summary>
        /// Represents a simpl class with no dependent simpl types to serialize in the typescope
        /// </summary>
        internal class SimplTestClassWithNoDependentSimplTypes
        {
            /// <summary>
            /// A string value for serialization
            /// </summary>
            public string SomeValue
            {
                get;
                set;
            }
        }

        /// <summary>
        /// A simple test case with fields and properties of all scopes
        /// </summary>
        internal class SimplTestClassWithFieldsAndPropertiesOfAllScopes
        {
            /// <summary>
            /// Gets or sets a public integer
            /// </summary>
            [SimplScalar]
            public int pubInt { get; set; }

            /// <summary>
            /// Gets or sets a protected string
            /// </summary>
            [SimplScalar]
            protected string protString { get; set; }

            /// <summary>
            /// Gets or sets a private long 
            /// </summary>
            [SimplScalar]
            private long privLong { get; set; }

            /// <summary>
            /// Gets or sets an internal double
            /// </summary>
            [SimplScalar]
            internal double internalDouble { get; set; }

            /// <summary>
            /// Gets or sets a public float
            /// </summary>
            [SimplScalar]
            public float fieldFloatPub = 1.0f;

            /// <summary>
            /// Gets or sets a protected byte
            /// </summary>
            [SimplScalar]
            protected byte fieldByteProt = 1;

            /// <summary>
            /// Gets or sets a short private 
            /// </summary>            
            [SimplScalar]
            private short fieldShortPrivate = 3;

            /// <summary>
            /// Gets or sets an internal bool 
            /// </summary>
            [SimplScalar]
            internal bool fieldBoolInternal = true;
        }



//        [TestClass]
    /// <summary>
    /// Test class for TypeExtensions. This will be refactored slightly, thus it is commented out.
    /// These type extensions will support a slight refactoring to clean up typescope creation.
    ///  </summary>
        public class TypeExtensionTests
        {
  //          [TestMethod]
            /// <summary>
            /// AllDependent types should return all types that need to be added to the type scope
            /// </summary>
            public void TypeExtension_AllDepTypesIncludesFieldsPropertiesAndSelf()
            {
                var AllDepTypes = TypeExtensions.GetAllDependentTypes(typeof(SimplTestClassWithFieldsAndPropertiesOfAllScopes));

                
                var cdReal = ClassDescriptor.GetClassDescriptor(typeof(SimplTestClassWithFieldsAndPropertiesOfAllScopes));

                var cdRealObj = ClassDescriptor.GetClassDescriptor(new SimplTestClassWithFieldsAndPropertiesOfAllScopes());
                
                Assert.AreEqual(9, AllDepTypes.Count());
            }

            /// <summary>
            /// If we do not intend to serialize a class with simpl, it should not be added to the type scope.
            /// </summary>
            public void TypeExtension_GetsZeroOnClassesWithNoSimplFields()
            {
                var Zero = TypeExtensions.GetAllDependentSimplTypes(typeof(SimplTestClassWithNoDependentSimplTypes));

                Assert.AreEqual(0, Zero.Count(), "Should have Zero simpl dependent types because this will not SimplSerialize.");
            }


            //[TestMethod]
            /// <summary>
            /// Types we add to the scope should include the given type
            /// </summary>
            public void SimplTypesIncludesGivenType()
            {
                var dependentSimplTypes = TypeExtensions.GetAllDependentSimplTypes(typeof(SimplTestClassWithNoDependentSimplTypes));

                Assert.AreEqual(expected: 1, actual: dependentSimplTypes.Length, message: "Should only have a single dependent type; more or less detected");
                Assert.AreEqual(typeof(SimplTestClassWithNoDependentSimplTypes), dependentSimplTypes[0], "The dependent type should be equal to the given type.");
            }
        }
}
