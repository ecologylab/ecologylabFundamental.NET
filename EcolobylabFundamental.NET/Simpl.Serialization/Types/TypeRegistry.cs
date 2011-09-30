using System;
using System.Collections.Generic;
using ecologylab.serialization.types;
using ecologylab.serialization.types.scalar;

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
        ///     Maps to hold the Type with their right abstraction
        /// </summary>
        private static Dictionary<String, ScalarType> allTypes = new Dictionary<String, ScalarType>();

        /// <summary>
        ///     Array of supported types
        /// </summary>
        static Type[] BASIC_TYPES = 
        {
            typeof(StringType), 
            typeof(DoubleType), 
            typeof(IntType),
            typeof(FloatType),
            typeof(LongType),
            typeof(ParsedUriType),
            typeof(BooleanType),
            typeof(ScalarTypeType),
            typeof(EnumeratedType),
            typeof(RegexType),
            typeof(StringBuilderType)
        };

        /// <summary>
        ///     Static constructor register basic types 
        /// </summary>
        static TypeRegistry()
        {
            Register(BASIC_TYPES);
        }

        /// <summary>
        ///     Maps basic types 
        /// </summary>
        /// <param name="basicTypes"></param>
        public static void Register(Type[] basicTypes)
        {
            for (int i = 0; i < basicTypes.Length; i++)
            {
                Register(basicTypes[i]);
            }
        }

        /// <summary>
        ///     Maps basic types with their simple and type names. 
        /// </summary>
        /// <param name="thatClass"></param>
        public static void Register(Type thatClass)
        {
            ScalarType scalarType = (ScalarType)Activator.CreateInstance(thatClass);
            String typeName = scalarType.EncapsulatedType.Name;
            String simpleName = scalarType.GetType().Name;
            Register(simpleName, scalarType);
            Register(typeName, scalarType);

        }

        /// <summary>
        ///     Adds the mapping if one doesn't exists. 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="scalarType"></param>
        private static void Register(String typeName, ScalarType scalarType)
        {
            if (!allTypes.ContainsKey(typeName))
            {
                allTypes.Add(typeName, scalarType);
            }
        }

        /// <summary>
        ///     Gets the type of the input class name from the registry.
        ///     Returns null if the object is not present in registry or not supported. 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static ScalarType GetType(String className)
        {
            if (allTypes.ContainsKey(className))
                return allTypes[className];
            else return null;
        }

        /// <summary>
        ///     Gets the scalar type of the field from registry.
        ///     Returns null if the object is not present in registry or not supported. 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static ScalarType GetType(System.Reflection.FieldInfo field)
        {
            return GetType(field.FieldType);
        }

        /// <summary>
        ///     Gets the scalar type of the field from registry.
        ///     Returns null if the object is not present in registry or not supported. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ScalarType GetType(Type type)
        {
            return XmlTools.IsEnum(type) ? GetType(typeof(Enum).Name) : GetType(type.Name);
        }

        public static void RegisterSimplType(SimplType simplType)
        {
            throw new NotImplementedException();
        }
    }
}
