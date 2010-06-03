using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.types.scalar
{
    /// <summary>
    /// 
    /// </summary>
    class TypeRegistry
    {
        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<String, ScalarType> allTypes = new Dictionary<String, ScalarType>();
        
        /// <summary>
        /// 
        /// </summary>
        static Type[] BASIC_TYPES = 
        {
            typeof(StringType), 
            typeof(DoubleType), 
            typeof(IntType),
            typeof(FloatType)
        };

        /// <summary>
        /// 
        /// </summary>
        static TypeRegistry()
        {
            Register(BASIC_TYPES);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicTypes"></param>
        private static void Register(Type[] basicTypes)
        {
            for (int i = 0; i < basicTypes.Length; i++)
            {
                Register(basicTypes[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        private static void Register(Type thatClass)
        {
            ScalarType scalarType = (ScalarType)Activator.CreateInstance(thatClass);
            String typeName = scalarType.EncapsulatedType.Name;
            String simpleName = scalarType.GetType().Name;
            Register(simpleName, scalarType);
            Register(typeName, scalarType);

        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private static ScalarType GetType(String className)
        {
            if (allTypes.ContainsKey(className))
                return allTypes[className];
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static ScalarType GetType(System.Reflection.FieldInfo field)
        {
            return GetType(field.FieldType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ScalarType GetType(Type type)
        {
            return GetType(type.Name);
        }
    }
}
