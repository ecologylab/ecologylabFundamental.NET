using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Simpl.Serialization.PlatformSpecifics
{
    class FundamentalPlatformSpecificsCSharp : IFundamentalPlatformSpecifics
    {
        // in ecologylab.serialization.ClassDescriptor;
        public void DeriveSuperClassGenericTypeVars(ClassDescriptor classDescriptor)
        {
		    Type describedClass = classDescriptor.DescribedClass;

		    if (describedClass == null)
			    return;

		    Type superClassType = describedClass.BaseType;

		    if (superClassType.IsGenericType)
		    {
			    classDescriptor.SuperClassGenericTypeVars = GetGenericTypeVars(superClassType, classDescriptor.GetGenericTypeVars());
		    }
        }

        // in ecologylab.serialization.FieldDescriptor;
        public void DeriveFieldGenericTypeVars(FieldDescriptor fieldDescriptor)
        {
		    FieldInfo field = fieldDescriptor.Field;
		    Type genericType = field.FieldType;
		    List<GenericTypeVar> derivedGenericTypeVars = new List<GenericTypeVar>();

		    if (genericType.IsGenericParameter)
		    {
			    GenericTypeVar g = GenericTypeVar.GetGenericTypeVarRef(genericType, fieldDescriptor.GetGenericTypeVarsContext());
			    derivedGenericTypeVars.Add(g);
		    }
		    else if (genericType.IsGenericType)
		    {
			    Type[] types = genericType.GetGenericArguments();

			    if (types == null | types.Length <= 0)
				    return;

			    foreach (Type t in types)
			    {
				    GenericTypeVar g = GenericTypeVar.GetGenericTypeVarRef(t, fieldDescriptor.GetGenericTypeVarsContext());
				    derivedGenericTypeVars.Add(g);
			    }
		    }
		
		    fieldDescriptor.SetGenericTypeVars(derivedGenericTypeVars);
        }

	    public static List<GenericTypeVar> GetGenericTypeVars(Type parameterizedType, List<GenericTypeVar> scope)
	    {
		    Type[] types = parameterizedType.GetGenericArguments();

		    if (types == null | types.Length <= 0)
			    return null;

		    List<GenericTypeVar> returnValue = new List<GenericTypeVar>();
		    foreach (Type t in types)
		    {
			    GenericTypeVar g = GenericTypeVar.GetGenericTypeVarRef(t, scope);
			    returnValue.Add(g);
		    }

		    return returnValue;
	    }

        public Type GetTypeArgClass(FieldInfo field, int i, FieldDescriptor fiedlDescriptor)
        {
		    Type result = null;

		    Type[] typeArgs; 
            
            Type realFieldType = field.FieldType;

            while (!realFieldType.IsGenericType)
            {
                realFieldType = realFieldType.BaseType;
            }

            typeArgs = realFieldType.GetGenericArguments();


            if (typeArgs != null)
		    {
			    int max = typeArgs.Length - 1;
			    if (i > max)
				    i = max;
			    Type typeArg0 = typeArgs[i];

                // case 1: arg is a concrete class
			    if (!typeArg0.IsGenericParameter && !typeArg0.IsGenericType) 
			    {
				    result = typeArg0;
			    }
			    else if (typeArg0.IsGenericType)
			    {
				    // nested parameterized type
				    
				    result = typeArg0.GetGenericTypeDefinition();
			    }
			    else if (typeArg0.IsGenericParameter)
			    {
                    Type[] tviBounds = typeArg0.GetGenericParameterConstraints();
				    result = tviBounds[0];
				    Console.Out.WriteLine( "yo! " + result);
			    }
			    else
			    {
				    Console.Out.WriteLine("getTypeArgClass(" + field + ", " + i
						    + " yucky! Consult s.im.mp serialization developers.");
			    }
		    }

		    return result;
        }

        // in ecologylab.serialization.GenericTypeVar;
        public void CheckBoundParameterizedTypeImpl(GenericTypeVar g, Type bound)
        {
		    if (bound.IsGenericType)
		    {
			    g.ConstraintClassDescriptor = ClassDescriptor.GetClassDescriptor(bound);

			    Type[] types = bound.GetGenericArguments();

			    foreach (Type type in types)
			    {
				    g.AddContraintGenericTypeVarArg(GenericTypeVar.GetGenericTypeVarRef(type, g.Scope));
			    }
		    }
        }

        public void CheckTypeParameterizedTypeImpl(GenericTypeVar g, Type type)
        {
		    if (type.IsGenericType)
		    {
			    g.ClassDescriptor = ClassDescriptor.GetClassDescriptor(type);

			    Type[] types = type.GetGenericArguments();

			    foreach (Type t in types)
			    {
				    g.AddGenericTypeVarArg(GenericTypeVar.GetGenericTypeVarRef(t, g.Scope));
			    }
		    }
        }
    }
}
