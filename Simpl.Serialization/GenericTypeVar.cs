using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.PlatformSpecifics;

namespace Simpl.Serialization
{
    /// <summary>
    ///<para>
    /// This class encapsulates generic type variables declarations on classes and fields.
    /// Different uses of this class:
    ///</para>
    /// <para>
    /// When used for definition before 'extends' (in ClassDescriptor): name + constraintClassDescriptor
    /// (+ constraintGenericTypeVarArgs) | constraintGenericTypeVar:
    /// </para>
    /// <para>
    /// name: the name of the new generic type var,
    /// </para>
    /// <para>
    /// constraintClassDescriptor: when the constraint is a concrete class, this holds the class
    /// descriptor;
    /// </para>
    /// <para>
    /// constraintGenericTypeVar: when the constraint is another generic type var, this refers to the
    /// definition of that generic type var;
    /// </para>
    /// <para>
    /// constraintGenericTypeVarArgs: when the constraint is parameterized, this holds type arguments.
    /// </para>
    /// <para>
    /// When used with field types (in FieldDescriptor):
    /// <para>
    /// <para>if the field is purely generic, name + referredGenericTypeVar:
    /// name: the generic type var name used as the type,
    /// referredGenericTypeVar: refers to the definition of this generic type var in class definition.</para>
    /// <para>if the field is parameterized, the FieldDescriptor should already have the class part of the
    /// field type, and its genericTypeVars field should have a list of type arguments. each type
    /// argument has classDescriptor / referredGenericTypeVar:
    /// classDescriptor: if the argument is a concrete class,
    /// referredGenericTypeVar: if the argument is parameterized or another generic type var. it should
    /// refer to the definition of that generic type var in class definition.</para>
    /// </para>
    /// </para>
    /// 
    /// <para>
    /// When used with base type after 'extends' (in ClassDescriptor): similar to the parameterized case
    /// when it is used for field type.
    /// </para>
    /// 
    /// @author quyin
    /// 
    /// </summary>

    public class GenericTypeVar
    {

        // The declared name of the generic type variable. such as 'M' for Media<M> test; 
        // The classDescriptor will be null if this parameter is populated. 
        [SimplScalar]
        String name;

        // This variable holds the ClassDecriptor of the class declared as a constraint to the generic type variable. 
        // e.g. for M, this holds ClassDescriptor<Media> in class MediaSearchResult<M extends Media>.
        [SimplComposite]
        ClassDescriptor constraintClassDescriptor = null;

        // This variable holds the generic type variable as the constraint.
        // e.g. for T1, this holds a reference to the definition of T in class MyClass<T1 extends T>. 
        [SimplComposite]
        GenericTypeVar constraintGenericTypeVar = null;

        // This variable holds the args of generic type variables of a parameterized constraint. 
        // e.g. for M, this holds references to definitions of R & S in class MediaSearchResult<M extends Media<R,S>>.
        [SimplCollection("generic_type_var")]
        List<GenericTypeVar> constraintGenericTypeVarArgs = null;

        // ClassDescriptor of the type arg. Not used for defining a new generic type var.
        // e.g. ClassDescriptor<Media> in MediaSearchResult<Media>; 
        [SimplComposite]
        ClassDescriptor classDescriptor = null;

        // If the type arg is parameterized, this holds the type arguments. each element in this collection
        // should have a name and a reference to the definition of that generic type var used as arg.
        // e.g. M & T in MediaSearchResult<Media<M,T>> (in this case the field classDescriptor should be ClassDescriptor<MediaSearchResult>) 
        [SimplCollection("generic_type_var")]
        List<GenericTypeVar> genericTypeVarArgs = null;

        // Refers to another generic type var, typically the definition. 
        // e.g. the 2nd M in class MediaSearchResult<M extends Media, M> (that GenericTypeVar object will have name=M and referredGenericTypeVar to the 1st M) 
        // may be used in other cases. see the javadoc of this class.
        [SimplComposite]
        GenericTypeVar referredGenericTypeVar = null;

        List<GenericTypeVar> scope;

        public GenericTypeVar()
        {
            //for simpl de/serialzation
        }

        public String Name
        {
            get { return name; }
            set { this.name = value; }
        }

        public ClassDescriptor ConstraintClassDescriptor
        {
            get { return constraintClassDescriptor; }
            set { this.constraintClassDescriptor = value; }
        }

        public GenericTypeVar ConstraintGenericTypeVar
        {
            get { return constraintGenericTypeVar; }
            set { this.constraintGenericTypeVar = value; }
        }

        public List<GenericTypeVar> ConstraintGenericTypeVarArgs
        {
            get { return constraintGenericTypeVarArgs; }
        }

        public void AddContraintGenericTypeVarArg(GenericTypeVar g)
        {
            if (constraintGenericTypeVarArgs == null)
                constraintGenericTypeVarArgs = new List<GenericTypeVar>();

            constraintGenericTypeVarArgs.Add(g);
        }

        public ClassDescriptor ClassDescriptor
        {
            get { return classDescriptor; }
            set { this.classDescriptor = value; }
        }

        public List<GenericTypeVar> GenericTypeVarArgs
        {
            get { return genericTypeVarArgs; }
        }

        public void AddGenericTypeVarArg(GenericTypeVar arg)
        {
            if (genericTypeVarArgs == null)
                genericTypeVarArgs = new List<GenericTypeVar>();

            genericTypeVarArgs.Add(arg);
        }

        public GenericTypeVar ReferredGenericTypeVar
        {
            get { return referredGenericTypeVar; }
            set { this.referredGenericTypeVar = value; }
        }

        public List<GenericTypeVar> Scope
        {
            get { return scope; }
        }

        /// <summary>
        /// Creates a GenericTypeVar object as the definition of a new generic type var, from a java
        /// reflection TypeVariable object.
        ///
        /// <param name='typeVariable'></param>
        /// <param name='scope'>the scope of current generic type vars; used to resolve generic type var names.</param>
        /// </summary>
        public static GenericTypeVar GetGenericTypeVarDef(Type typeVariable, List<GenericTypeVar> scope)
        {
            GenericTypeVar g = new GenericTypeVar();
            g.scope = scope;
            g.name = typeVariable.Name;

            // resolve constraints
            ResolveGenericTypeVarDefinitionConstraints(g, typeVariable.GetGenericParameterConstraints());

            g.scope = null;
            return g;
        }

        /// <sumary>
        /// Creates a GenericTypeVar object as in a type reference (usage), from a java reflection Type
        /// object.
        /// <sumary>
        public static GenericTypeVar GetGenericTypeVarRef(Type type, List<GenericTypeVar> scope)
        {
            GenericTypeVar g = new GenericTypeVar();
            g.scope = scope;

            // case 1: arg is a concrete class
            if (!type.IsGenericParameter && !type.IsGenericType)
            {
                g.classDescriptor = ClassDescriptor.GetClassDescriptor(type);
                return g;
            }
            else if (type.IsGenericParameter)// case 2: arg is another generic type var
            {
                String argName = type.Name;
                if (argName != null && scope != null)
                {
                    foreach (GenericTypeVar var in scope)
                        if (argName.Equals(var.Name))
                        {
                            g.name = var.Name;
                            g.referredGenericTypeVar = var;
                            break;
                        }
                }
            }

            // case 3: arg is parameterized
            checkTypeParameterizedTypeImpl(g, type);
		
            g.scope = null;
            return g;
        }
        
        /// <sumary>
        /// Resolves constraints on the definition of a generic type var.
        /// <sumary>
        public static void ResolveGenericTypeVarDefinitionConstraints(GenericTypeVar g, Type[] bounds)
        {
            if (bounds == null)
                return;

            Type bound = bounds[0];

            // case 1: constraint is a concrete class
            if (!bound.IsGenericParameter)
            {
                if (typeof(Object) != bound)
                    g.constraintClassDescriptor = ClassDescriptor.GetClassDescriptor(bound);
            }
            else // case 2: constraint is another generic type var
            {
                // look up the scope to find the bound generic type var (must have been defined)
                String boundName = bound.Name;
                if (boundName != null && g.scope != null)
                {
                    foreach (GenericTypeVar var in g.scope)
                        if (boundName.Equals(var.Name))
                        {
                            g.constraintGenericTypeVar = var;
                            break;
                        }
                }
            }

            // case 3: constraint is parameterized -- the most complicated case
            checkBoundParameterizedTypeImpl(g, bound);
        }

        /// <sumary>
        /// Resolves constraints on a generic type var that is used in a type reference (usage).
        /// <sumary>
        public static void ResolveGenericTypeVarReferenceConstraints(GenericTypeVar g, Type[] bounds)
        {
            if (bounds == null)
                return;

            Type bound = bounds[0];

            // case 1: constraint is a concrete class
            if (!bound.IsGenericParameter)
            {
                if (typeof(Object) != bound)
                    g.ClassDescriptor = ClassDescriptor.GetClassDescriptor(bound);
            }
            else // case 2: constraint is another generic type var
            {
                String boundName = bound.Name;
                if (boundName != null && g.scope != null)
                {
                    foreach (GenericTypeVar var in g.scope)
                        if (boundName.Equals(var.Name))
                        {
                            g.ConstraintGenericTypeVar = var;
                            break;
                        }
                }
            }
		
            // case 3: constraint is parameterized -- the most complicated case
            checkBoundParameterizedTypeImpl(g, bound);
        }
        
        public static void checkBoundParameterizedTypeImpl (GenericTypeVar g, Type bound)
        {
            FundamentalPlatformSpecifics.Get().CheckBoundParameterizedTypeImpl(g, bound);
        }
		
        public static void checkTypeParameterizedTypeImpl(GenericTypeVar g, Type type)
        {
            FundamentalPlatformSpecifics.Get().CheckTypeParameterizedTypeImpl(g, type);
        }
        
        public bool IsDef()
        {
            return name != null && name.Length > 0 && (constraintClassDescriptor != null || constraintGenericTypeVar != null) && referredGenericTypeVar == null;
        }

        public override string ToString()
        {
            {
                StringBuilder sb = new StringBuilder();

                if (IsDef())
                {
                    sb.Append(name);
                    if (constraintGenericTypeVar != null)
                    {
                        sb.Append(" : ").Append(constraintGenericTypeVar.name);
                    }
                    else if (constraintClassDescriptor != null)
                    {
                        sb.Append(" : ").Append(constraintClassDescriptor.DescribedClassSimpleName);
                        if (constraintGenericTypeVarArgs != null && constraintGenericTypeVarArgs.Count > 0)
                        {
                            sb.Append("<");
                            for (int i = 0; i < constraintGenericTypeVarArgs.Count; ++i)
                            {
                                GenericTypeVar g = constraintGenericTypeVarArgs[i];
                                sb.Append(i == 0 ? "" : ",").Append(g.ToString());
                            }
                            sb.Append(">");
                        }
                    }
                }
                else
                {
                    if (name != null || referredGenericTypeVar != null)
                    {
                        sb.Append(name);
                    }
                    else if (classDescriptor != null)
                    {
                        sb.Append(classDescriptor.DescribedClassSimpleName);
                        if (genericTypeVarArgs != null && genericTypeVarArgs.Count > 0)
                        {
                            sb.Append("<");
                            for (int i = 0; i < genericTypeVarArgs.Count; ++i)
                            {
                                GenericTypeVar g = genericTypeVarArgs[i];
                                sb.Append(i == 0 ? "" : ",").Append(g.ToString());
                            }
                            sb.Append(">");
                        }
                    }
                }
                //		
                //		if (name != null && name.length() > 0)
                //			sb.append(name);
                //
                //		if (name != null && name != "")
                //			sb += name;
                //		else if (classDescriptor != null)
                //			sb += classDescriptor.getDescribedClassSimpleName();
                //		
                //		if (genericTypeVarArgs != null)
                //		{
                //			for (GenericTypeVar g : genericTypeVarArgs)
                //			{
                //				sb += "<";
                //				sb += g.toString();
                //				sb += ">";
                //			}
                //		}
                //		
                //		if (constraintClassDescriptor != null || constraintGenericTypeVarArgs != null)
                //			sb += " extends ";
                //
                //		if (constraintClassDescriptor != null)
                //			sb += constraintClassDescriptor.getDescribedClassSimpleName();
                //
                //		if (constraintGenericTypeVarArgs != null)
                //		{
                //			for (GenericTypeVar g : constraintGenericTypeVarArgs)
                //			{
                //				sb += "<";
                //				sb += g.toString();
                //				sb += ">";
                //			}
                //		}

                return sb.ToString();
            }
        }
    }
}
