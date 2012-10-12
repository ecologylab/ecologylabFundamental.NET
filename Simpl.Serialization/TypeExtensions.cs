namespace Simpl.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Simpl.Serialization.Attributes;

    /// <summary>
    /// Adds override to a given Type to get all requisite types for a serialization type scope. 
    /// Adding an extension method to Type feels daring and dangerous, but it can improve the readability of some of our code. 
    /// Will help replace some declarations that look like this: 
    /// typeof(ClassA), typeof(ClassA_DependentType), typeof(ClassA_AnotherDependentType)
    /// with just
    /// typeof(ClassA) 
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns all types that this type "Depends" upon, filtered by a member info predicate (to filter for attributes, etc) 
        /// </summary>
        public static IEnumerable<Type> GetAllDependentTypes(this Type currentType, Func<MemberInfo, bool> matchesPredicate)
        {
            return GetDependentTypesWithDuplicates(currentType, matchesPredicate).Distinct();
        }

        /// <summary>
        /// Determines if this currentType can be assigned to the TargetType
        /// </summary>
        /// <typeparam name="TargetType">Type to asign to</typeparam>
        /// <param name="currentType">Our current Type</param>
        /// <returns>True if so (If the targetType is assignable from the current type, the current type is assignable TO the target type)</returns>
        public static bool IsAssignableTo<TargetType>(this Type currentType)
        {
            return typeof(TargetType).GetTypeInfo().IsAssignableFrom(currentType.GetTypeInfo());
        }

        private static IEnumerable<Type> GetDependentTypesWithDuplicates(Type currentType, Func<MemberInfo, bool> matchesPredicate)
        {
            // Check over all Properties first. 

            //BindingFlags bindingParameters = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance;


            foreach (var propInfo in currentType.GetTypeInfo().DeclaredProperties)
            {
                if (matchesPredicate(propInfo))
                {
                    yield return propInfo.PropertyType;
                }
            }

            // check over all fields next

            foreach (var fieldInfo in currentType.GetTypeInfo().DeclaredFields)
            {
                if (matchesPredicate(fieldInfo))
                {
                    yield return fieldInfo.FieldType;
                }
            }

            yield return currentType;
        }

        /// <summary>
        /// Returns all types that this type "Depends" upon. These types are the types of fields and properties of the given type. 
        /// </summary>
        /// <param name="currentType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllDependentTypes(this Type currentType)
        {
            return currentType.GetAllDependentTypes(matchesPredicate: memberInfo => true); // Fetch all members with a predicate that always returns true 
        }

        public static Type[] GetAllDependentSimplTypes(this Type currentType)
        {
            return currentType.GetAllDependentTypes(matchesPredicate: member => member.IsSimplMember()).ToArray();
        }

        private static Type[] simplAttributes = new[] { typeof(SimplScalar), typeof(SimplCollection), typeof(SimplComposite), typeof(SimplCompositeAsScalar) };

        private static bool MemberHasSimplAttributes<T>(T member) where T : MemberInfo
        {
            return member.GetCustomAttributes(true).Where(attrObj => simplAttributes.Contains(attrObj.GetType())).Any();
        }

        /// <summary>
        /// If a member is a Simpl type (scalar, collection, etc.)
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsSimplMember(this MemberInfo member)
        {
            return MemberHasSimplAttributes(member);
        }
    }
}
