using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        /// 
        /// </summary>
        private static TypeRegistry scalarRegistry;

        /// <summary>
        /// 
        /// </summary>
        private static TypeRegistry collectionRegistry;

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, SimplType> _typesByJavaName = new Dictionary<String, SimplType>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, SimplType> _typesByCrossPlatformName = new Dictionary<String, SimplType>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, SimplType> _typesBySimpleName = new Dictionary<String, SimplType>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, SimplType> _typesBycSharpName = new Dictionary<String, SimplType>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, SimplType> _typesByObjectiveCName = new Dictionary<String, SimplType>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, SimplType> _typesByDbName = new Dictionary<String, SimplType>();

        /// <summary>
        /// 
        /// </summary>
        static TypeRegistry()
        {
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        private static Boolean isInit;

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            if (!isInit)
            {
                isInit = true;

                new FundamentalTypes();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static TypeRegistry ScalarRegistry
        {
            get
            {
                TypeRegistry result = scalarRegistry;

                if (result == null)
                {
                    lock (typeof (TypeRegistry))
                    {
                        result = scalarRegistry;
                        if (result == null)
                        {
                            result = new TypeRegistry();
                            scalarRegistry = result;
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static TypeRegistry CollectionRegistry
        {
            get
            {
                TypeRegistry result = collectionRegistry;
                if (result == null)
                {
                    lock (typeof (TypeRegistry))
                    {
                        result = collectionRegistry;
                        if (result == null)
                        {
                            result = new TypeRegistry();
                            collectionRegistry = result;
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Boolean RegisterSimplType(SimplType type)
        {
            if (type is CollectionType)
            {
                return CollectionRegistry.RegisterType(type);
            }

            if (type is ScalarType)
                return ScalarRegistry.RegisterType(type);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Boolean RegisterType(SimplType type)
        {

            _typesBycSharpName.Add(type.CSharpTypeName, type);
            _typesByCrossPlatformName.Add(type.Name, type);

            if (type.JavaTypeName != null)
                _typesByJavaName.Add(type.JavaTypeName, type);

            if (type.ObjectiveCTypeName != null)
                _typesByObjectiveCName.Add(type.ObjectiveCTypeName, type);

            if (type.DbTypeName != null)
                _typesByDbName.Add(type.DbTypeName, type);

            if (_typesBySimpleName.ContainsKey(type.SimplName))
            {
                Debug.WriteLine("registerType(): Redefining type: " + type.SimplName);
                _typesBySimpleName.Remove(type.SimplName);
                _typesBySimpleName.Add(type.SimplName, type);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Boolean RegisterScalarType(ScalarType type)
        {
            return ScalarRegistry.RegisterType(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Boolean RegisterTypeIfNew(SimplType type)
        {
            String javaTypeName = type.JavaTypeName;
            return !_typesByJavaName.ContainsKey(javaTypeName) && RegisterType(type);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatType"></param>
        /// <returns></returns>
        public static ScalarType GetScalarType(Type thatType)
        {
            if (XmlTools.IsEnum(thatType))
            {
                return (ScalarType) ScalarRegistry.GetTypeByType(typeof(Enum));
            }
            return (ScalarType) ScalarRegistry.GetTypeByType(thatType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Boolean ContainsScalarType(Type type)
        {
            return ScalarRegistry.Contains(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Boolean Contains(Type type)
        {
            return ContainsBycSharpName(type.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpName"></param>
        /// <returns></returns>
        private Boolean ContainsBycSharpName(string cSharpName)
        {
            return _typesBycSharpName.ContainsKey(cSharpName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simpleName"></param>
        /// <returns></returns>
        public static ScalarType GetScalarTypeBySimpleName(String simpleName)
        {
            return (ScalarType) ScalarRegistry.GetTypeBySimpleName(simpleName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simpleName"></param>
        /// <returns></returns>
        private SimplType GetTypeBySimpleName(String simpleName)
        {
            return _typesBySimpleName[simpleName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpType"></param>
        /// <returns></returns>
        public SimplType GetTypeByType(Type cSharpType)
        {
            return GetTypeBycSharpName(cSharpType.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpName"></param>
        /// <returns></returns>
        public SimplType GetTypeBycSharpName(String cSharpName)
        {
            return _typesBycSharpName[cSharpName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="javaName"></param>
        /// <returns></returns>
        public SimplType GetTypeByJavaName(String javaName)
        {
            return _typesByJavaName[javaName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectiveCName"></param>
        /// <returns></returns>
        public SimplType GetTypeByObjectiveCName(String objectiveCName)
        {
            return _typesByObjectiveCName[objectiveCName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public SimplType GetTypeByDbName(String dbName)
        {
            return _typesByDbName[dbName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionType"></param>
        public static void RegisterCollectionType(CollectionType collectionType)
        {
            CollectionRegistry.RegisterType(collectionType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crossPlatformName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeByCrossPlatformName(String crossPlatformName)
        {
            return (CollectionType) CollectionRegistry._typesByCrossPlatformName[crossPlatformName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeByCSharpName(String cSharpName)
        {
            return (CollectionType) CollectionRegistry._typesBycSharpName[cSharpName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectiveCName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeByObjectiveCName(String objectiveCName)
        {
            return (CollectionType)CollectionRegistry._typesByObjectiveCName[objectiveCName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simpleName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeBySimpleName(String simpleName)
        {
            return (CollectionType) CollectionRegistry._typesBySimpleName[simpleName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpField"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionType(FieldInfo cSharpField)
        {
            return GetCollectionType(cSharpField.FieldType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpFieldType"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionType(Type cSharpFieldType)
        {
            String cSharpName = cSharpFieldType.FullName;
            CollectionType result = GetCollectionTypeBycSharpName(cSharpName);
            if (result == null)
            {
                if (cSharpFieldType.IsInterface || cSharpFieldType.IsAbstract)
                {
                    return cSharpFieldType is IDictionary
                               ? collectionRegistry.DefaultMapType
                               : collectionRegistry.DefaultCollectionType;
                }
                String crossPlatformName = SimplType.DeriveCrossPlatformName(cSharpFieldType, false);
                Debug.WriteLine("No CollectionType was pre-defined for " + crossPlatformName +
                                  ", so constructing one on the fly.\nCross-language code for fields defined with this type cannot be generated.");
                result = new CollectionType(cSharpFieldType, null, null);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpClassName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeBycSharpName(String cSharpClassName)
        {
            return (CollectionType) collectionRegistry._typesBycSharpName[cSharpClassName];
        }

        /// <summary>
        /// 
        /// </summary>
        public static TypeRegistry ScalarTypeRegistry
        {
            get { return scalarRegistry; }
        }

        /// <summary>
        /// 
        /// </summary>
        public CollectionType DefaultCollectionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CollectionType DefaultMapType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDictionary"></param>
        /// <returns></returns>
        public static CollectionType GetDefaultCollectionOrMapType(Boolean isDictionary)
        {
            return isDictionary ? CollectionRegistry.DefaultMapType : CollectionRegistry.DefaultCollectionType;
        }
    }
}