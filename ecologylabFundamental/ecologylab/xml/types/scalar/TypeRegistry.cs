using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.types.scalar
{
    class TypeRegistry
    {

        private static Dictionary<String, ScalarType> allTypes = new Dictionary<String, ScalarType>();
        
        static Type[] BASIC_TYPES = 
        {
            typeof(StringType), 
            typeof(DoubleType), 
            typeof(IntType),
            typeof(FloatType)
        };

        static TypeRegistry()
        {
            Register(BASIC_TYPES);
        }

        private static void Register(Type[] basicTypes)
        {
            for (int i = 0; i < basicTypes.Length; i++)
            {
                Register(basicTypes[i]);
            }
        }

        private static void Register(Type thatClass)
        {
            ScalarType scalarType = (ScalarType)Activator.CreateInstance(thatClass);
            String typeName = scalarType.EncapsulatedType.Name;
            String simpleName = scalarType.GetType().Name;
            Register(simpleName, scalarType);
            Register(typeName, scalarType);

        }

        private static void Register(String typeName, ScalarType scalarType)
        {
            if (!allTypes.ContainsKey(typeName))
            {
                allTypes.Add(typeName, scalarType);
            }
        }

        private static ScalarType GetType(String className)
        {
            if (allTypes.ContainsKey(className))
                return allTypes[className];
            else return null;
        }


        public static ScalarType GetType(System.Reflection.FieldInfo field)
        {
            return GetType(field.FieldType);
        }

        internal static ScalarType GetType(Type type)
        {
            return GetType(type.Name);
        }
    }
}
