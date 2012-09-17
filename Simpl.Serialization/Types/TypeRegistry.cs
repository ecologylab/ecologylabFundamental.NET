using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Simpl.Fundamental.Generic;
using System.Diagnostics.Contracts;

namespace Simpl.Serialization.Types
{
    /// <summary>
    ///     Class to hold the mappings of different datatypes 
    ///     supported by the serialization framework. This objects 
    ///     returns the right abstraction for the supported types. 
    /// </summary>
    public class TypeRegistry
    {
        /// <summary>
        /// Collection of scalar types contained in this type registry
        /// </summary>
        private static TypeCollection<ScalarType> scalarTypeCollection = new TypeCollection<ScalarType>();

        /// <summary>
        /// Collection of collectionTypes contained in this type registry
        /// </summary>
        private static TypeCollection<CollectionType> collectionTypeCollection = new TypeCollection<CollectionType>((t) => new CollectionType(t,t.Name,t.Name));

        /// <summary>
        /// Collection of composite types contained in this type registry
        /// </summary>
        private static TypeCollection<CompositeType> compositeTypeCollection = new TypeCollection<CompositeType>((t) => new CompositeType(t));

        /// <summary>
        /// Collection of all simpl types contained in this type registry
        /// </summary>
        private static TypeCollection<SimplType> simplTypeCollection = new TypeCollection<SimplType>();

        public static TypeCollection<ScalarType> ScalarTypes
        {
            get
            {
                return scalarTypeCollection;
            }
        }

        public static TypeCollection<CollectionType> CollectionTypes
        {
            get
            {
                return collectionTypeCollection;
            }
        }

        public static TypeCollection<CompositeType> CompositeTypes
        {
            get
            {
                return compositeTypeCollection;
            }
        }

        public static TypeCollection<SimplType> SimplTypes
        {
            get
            {
                return simplTypeCollection;
            }
        }

        /// <summary>
        /// Initializes the TypeRegistry
        /// </summary>
        static TypeRegistry()
        {
            Init();
        }

        /// <summary>
        /// Gets a value indicating if the TypeRegistry has been initialized
        /// </summary>
        private static Boolean isInit;

        /// <summary>
        /// Initializes the TypeRegistry by adding all fundamental types to the Registry.
        /// </summary>
        public static void Init()
        {
            if (!isInit)
            {
                // Add fundamental types to the TypeRegistry by creating all fundamental simpl types.
                // (Every time a simpl_type object is created, it gets enrolled in the registry statically)
                new FundamentalTypes();

                // We should always have scalar types registered... 
                Contract.Requires(TypeRegistry.ScalarTypes.CSharpType.Count > 0, "Scalar types not added to TypeRegistry upon static initialization.");
                
                // We have initailized the TypeRegistry
                isInit = true;
            }
        }

        /// <summary>
        /// Registers a given SimplType into one of the relevant type registries.
        /// </summary>
        /// <param name="type">Type to register</param>
        /// <returns>True if it succeded, false if it was not registered, exception if an issue occurred or the type would not fit in a category.</returns>
        public static bool RegisterSimplType(SimplType type)
        {
            bool result = false;

            result |= ScalarTypes.TryAdd(type);
            result |= CollectionTypes.TryAdd(type);
            result |= CompositeTypes.TryAdd(type);
            result |= SimplTypes.TryAdd(type);
        
            return result;

        }

        /// <summary>
        /// Registers an entire array of simplTypes
        /// </summary>
        /// <param name="simplTypes">Simpl Types to register</param>
        /// <returns>True if all succeeded, false if any fail.</returns>
        public static bool RegisterTypes(SimplType[] simplTypes)
        {
            bool result = true;

            foreach (var type in simplTypes)
            {
                result &= RegisterSimplType(type);
            }

            return result;
        }

        //TODO: SIMPL WILL NOT HANDLE MULTIPLE ENUM TYPES IN A GIVEN SCOPE PROPERLY... I THINK. :P

        /// <summary>
        /// 
        /// </summary>
        public static CollectionType DefaultCollectionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static CollectionType DefaultMapType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDictionary"></param>
        /// <returns></returns>
        public static CollectionType GetDefaultCollectionOrMapType(Boolean isDictionary)
        {
            return isDictionary ? DefaultMapType : DefaultCollectionType;
        }

        public static void SetDefaultCollectionType(CollectionType ct)
        {
            DefaultCollectionType = ct;
        }

        public static void SetDefaultMapType(CollectionType ct)
        {
            DefaultMapType = ct;
        }

        internal static CollectionType GetCollectionType(FieldInfo thatField)
        {
            if (CollectionType.CanBeCreatedFrom(thatField.FieldType))
            {
                var created = new CollectionType(thatField.FieldType, javaName: thatField.Name, objCName: thatField.Name);
                compositeTypeCollection.TryAdd(created);
                return created;
            }
            else
            {
                throw new ArgumentException(String.Format("Field type is not a legitimate collection we can create. Type: {0}", thatField.FieldType));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpClassName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeBycSharpName(String cSharpClassName)
        {
            return CollectionTypes.CSharpName[cSharpClassName];
        }
    }
}