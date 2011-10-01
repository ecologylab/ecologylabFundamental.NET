using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Simpl.Serialization.Types
{
    /// <summary>
    ///     Class to hold the mappings of different datatypes 
    ///     supported by the serialization framework. This objects 
    ///     returns the right abstraction for the supported types. 
    /// </summary>
    public class TypeRegistry<T> where T : SimplType
    {

        /// <summary>
        /// 
        /// </summary>
        private static TypeRegistry<ScalarType> scalarRegistry;

        /// <summary>
        /// 
        /// </summary>
        private static TypeRegistry<CollectionType> collectionRegistry;

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, T> _typesByJavaName = new Dictionary<String, T>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, T> _typesByCrossPlatformName = new Dictionary<String, T>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, T> _typesBySimpleName = new Dictionary<String, T>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, T> _typesBycSharpName = new Dictionary<String, T>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, T> _typesByObjectiveCName = new Dictionary<String, T>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, T> _typesByDbName = new Dictionary<String, T>();

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
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static TypeRegistry<ScalarType> ScalarRegistry
        {
            get
            {
                TypeRegistry<ScalarType> result = scalarRegistry;

                if (result == null)
                {
                    lock (typeof (TypeRegistry<T>))
                    {
                        result = scalarRegistry;
                        if (result == null)
                        {
                            result = new TypeRegistry<ScalarType>();
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
        private static TypeRegistry<CollectionType> CollectionRegistry
        {
            get
            {
                TypeRegistry<CollectionType> result = collectionRegistry;
                if (result == null)
                {
                    lock (typeof (TypeRegistry<T>))
                    {
                        result = collectionRegistry;
                        if (result == null)
                        {
                            result = new TypeRegistry<CollectionType>();
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
                return CollectionRegistry.RegisterType((CollectionType) type);
            }

            if (type is ScalarType)
                return ScalarRegistry.RegisterType((ScalarType) type);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Boolean RegisterType(T type)
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
                Console.WriteLine("registerType(): Redefining type: " + type.SimplName);
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
        public Boolean RegisterTypeIfNew(T type)
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
                return ScalarRegistry.GetTypeByType(typeof (Enum));
            }

            return null;
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
            return ScalarRegistry.GetTypeBySimpleName(simpleName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simpleName"></param>
        /// <returns></returns>
        private T GetTypeBySimpleName(String simpleName)
        {
            return _typesBySimpleName[simpleName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpType"></param>
        /// <returns></returns>
        public T GetTypeByType(Type cSharpType)
        {
            return GetTypeBycSharpName(cSharpType.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpName"></param>
        /// <returns></returns>
        public T GetTypeBycSharpName(String cSharpName)
        {
            return _typesBycSharpName[cSharpName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="javaName"></param>
        /// <returns></returns>
        public T GetTypeByJavaName(String javaName)
        {
            return _typesByJavaName[javaName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectiveCName"></param>
        /// <returns></returns>
        public T GetTypeByObjectiveCName(String objectiveCName)
        {
            return _typesByObjectiveCName[objectiveCName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public T GetTypeByDbName(String dbName)
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
            return CollectionRegistry._typesByCrossPlatformName[crossPlatformName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeByCSharpName(String cSharpName)
        {
            return CollectionRegistry._typesBycSharpName[cSharpName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectiveCName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeByObjectiveCName(String objectiveCName)
        {
            return CollectionRegistry._typesByObjectiveCName[objectiveCName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simpleName"></param>
        /// <returns></returns>
        public static CollectionType GetCollectionTypeBySimpleName(String simpleName)
        {
            return CollectionRegistry._typesBySimpleName[simpleName];
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
                Console.WriteLine("No CollectionType was pre-defined for " + crossPlatformName +
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
            return collectionRegistry._typesBycSharpName[cSharpClassName];
        }

        /// <summary>
        /// 
        /// </summary>
        public static TypeRegistry<ScalarType> ScalarTypeRegistry
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