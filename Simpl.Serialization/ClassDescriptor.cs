using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.Serializers;
using ecologylab.serialization;
using Simpl.Serialization.PlatformSpecifics;

namespace Simpl.Serialization
{
    /// <summary>
    ///     <para>
    ///     <c>ClassDescriptors</c> are created once for each class.
    ///     They store the optimized data structures for marshalling and 
    ///     unmarshalling of classes to their XML representation. It provides
    ///     functionality to create class descriptors for iiBAM annotated C# 
    ///     class files. 
    ///     </para>
    ///     <para>
    ///     It also provides static methods to get class descriptors from the
    ///     global association for each class descriptor. It does lazy evaluation
    ///     of annotation meta-language.
    ///     </para>
    /// </summary>
    public class ClassDescriptor : DescriptorBase, IEnumerable<FieldDescriptor>
    {
        /// <summary>
        ///     Holds the <c>Type</c> of the class described by this 
        ///     <c>ClassDescriptor</c>
        /// </summary>
        private readonly Type _describedClass;

        /// <summary>
        ///     Holds the <c>String</c> simple name of the described class.
        /// </summary>
        private readonly String _describedClassSimpleName;

        /// <summary>
        /// 
        /// </summary>
        private String _describedClassPackageName;

        /// <summary>
        ///     An abstract <c>FieldDescriptor</c> for wrapped collections. This
        ///     only holds the tag name for the field since there is no field 
        ///     associated with the <c>FieldDescriptor</c>.
        /// </summary>
        private FieldDescriptor _pseudoFieldDescriptor;


        /// <summary>
        /// 
        /// </summary>
        private FieldDescriptor _scalarTextFD;

        private ClassDescriptor _superClass;

        /// <summary>
        ///     This flag indicates if the framework has completed resolving  
        ///     annotations for the class described by this <c>ClassDescriptor</c> 
        ///     or its super classes.
        /// </summary>
        private Boolean _isGetAndOrganizeComplete;


        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<String, FieldDescriptor> _allFieldDescriptorsByTagNames =
            new Dictionary<String, FieldDescriptor>();

        /// <summary>
        /// 
        /// </summary>
        private readonly List<FieldDescriptor> _attributeFieldDescriptors = new List<FieldDescriptor>();

        /// <summary>
        ///     List of <c>FieldDescriptors</c>, which are represented as leaf nodes 
        ///     when marshalled.
        /// </summary>
        private readonly List<FieldDescriptor> _elementFieldDescriptors = new List<FieldDescriptor>();

        [SimplNoWrap] [SimplMap("field_descriptor")] private readonly DictionaryList<String, FieldDescriptor>
            _fieldDescriptorsByFieldName = new DictionaryList<String, FieldDescriptor>();

        /// <summary>
        ///     Static dictionary containing all the <c>ClassDescriptors</c>, with their 
        ///     tagNames. This is used for fast access to class descriptors.
        /// </summary>
        private static readonly Dictionary<String, ClassDescriptor> GlobalClassDescriptorsMap =
            new Dictionary<String, ClassDescriptor>();

        private List<FieldDescriptor> _unresolvedScopeAnnotationFDs;

        private List<FieldDescriptor> _unresolvedClassesAnnotationFDs;

        private FieldDescriptor _scalarValueFieldDescriptor;

        //generics
        [SimplCollection("generic_type_variable")]
	    private List<String> genericTypeVariables = new List<String>();

	    [SimplCollection("generic_type_var")]
	    private List<GenericTypeVar> genericTypeVars = null;

	    private List<String> declaredGenericTypeVarNames	= null;
	
	    [SimplCollection("super_class_generic_type_var")]
	    private List<GenericTypeVar> superClassGenericTypeVars = null;

        /// <summary>
        /// defines whether a strict object graph is required based on the equality operator
        /// </summary>
        [SimplScalar] private bool _strictObjectGraphRequired = false;

        [SimplScalar] private Type fieldDescriptorClass;
        [SimplScalar] private Type classDescriptorClass;

        /// <summary>
        /// Whether a strict object graph is required
        /// </summary>
        public bool StrictObjectGraphRequired
        {
            get { return _strictObjectGraphRequired; }
            set { _strictObjectGraphRequired = value; }
        }

        /// <summary>
        ///     Constructor for <c>ClassDescriptor</c> takes
        ///     <c>Type</c> as the input parameter. Initializes
        ///     internal variables and resovles the tagName for 
        ///     class.
        /// </summary>
        /// <param name="thatClass">
        ///     <c>Type</c> of the class defined by this  
        ///     <c>ClassDescriptor</c>
        /// </param>
        private ClassDescriptor(Type thatClass)
            : base(XmlTools.GetXmlTagName(thatClass, SimplTypesScope.STATE), thatClass.Name)
        {
            _describedClass = thatClass;
            _describedClassSimpleName = thatClass.Name;
            _describedClassPackageName = thatClass.Namespace;

            //generics
            if (thatClass.IsGenericType)
            {
                int index = _describedClassSimpleName.IndexOf('`');
                String simpleName = _describedClassSimpleName.Substring(0, index);

                name = simpleName;
                _describedClassSimpleName = simpleName;

                index = _tagName.IndexOf('`'); 
                if (index != -1)
                {
                    String tagName = _tagName.Substring(0, index);

                    this._tagName = tagName;
                }
            }            
            
            if (XmlTools.IsAnnotationPresent<SimplUseEqualsEquals>(thatClass))
            {
                _strictObjectGraphRequired = true;
            }

            SimplDescriptorClasses descriptorsClassesAttribute = XmlTools.GetAnnotation<SimplDescriptorClasses>(thatClass);
            
            if(descriptorsClassesAttribute != null)
            {
                classDescriptorClass = descriptorsClassesAttribute.Classes[0];
                fieldDescriptorClass = descriptorsClassesAttribute.Classes[1];
            }
            
            //generics
            AddGenericTypeVariables();
        }

        /// <summary>
        ///     Constructor for initializing the 
        ///     <c>ClassDescriptor</c> with the <c>tag</c>
        /// </summary>
        /// <param name="tagName">
        ///     <c>String</c> tagName for the <c>ClassDescriptor</c>
        /// </param>
        private ClassDescriptor(String tagName, String comment, String describedClassPackageName,
                               String describedClassSimpleName, ClassDescriptor superClass, List<String> interfaces)
            : base(tagName, describedClassPackageName + "." + describedClassSimpleName, comment)
        {
            _describedClassPackageName = describedClassPackageName;
            _describedClassSimpleName = describedClassSimpleName;
            _superClass = superClass;

        }

        /// <summary>
        ///     Returns the <c>ClassDescriptor</c> associated with the class type.
        ///     Uses the global class descriptor map to fetch the <c>ClassDescriptor</c>.
        ///     If it is being for the first time it recusively generate <c>ClassDescriptors</c>
        ///     and resolve annotations.
        /// </summary>
        /// <param name="thatClass">
        ///     <c>Type</c> of the class
        /// </param>
        /// <returns>
        ///     <c>ClassDescripor</c> for the paramater class or <c>null</c> 
        ///     if there is no associated class descriptor.
        /// </returns>
        public static ClassDescriptor GetClassDescriptor(Type thatClass)
        {
            String className = thatClass.FullName;// for generic classes, className could be null!


            //generics
            while (thatClass.IsGenericParameter)//e.g. where X : Media \n where I : X \n ... List<I> field;  
            {
                Type[] thatClassConstraints = thatClass.GetGenericParameterConstraints();

                if (thatClassConstraints == null || thatClassConstraints.Length == 0)
                {
                    thatClass = typeof(Object);
                }
                else
                {
                    thatClass = thatClassConstraints[0];
                }

                className = thatClass.FullName;
            }

            if (thatClass.IsGenericType)//can also be a generic parameter that extends a generic type
            {
                if (thatClass.FullName == null)
                {
                    className = thatClass.GetGenericTypeDefinition().FullName;
                }
            }

            ClassDescriptor result;

            if (!GlobalClassDescriptorsMap.TryGetValue(className, out result))
            {
                var descriptorsAnnotation = XmlTools.GetAnnotation<SimplDescriptorClasses>(thatClass, considerInheritedAnnotations: true);

                if (descriptorsAnnotation == null)
                {
                    result = new ClassDescriptor(thatClass);
                }
                else
                {
                    //First class is the type of the class descriptor, the second the type of the fieldDescriptor.
                    Type classDescriptorClass = descriptorsAnnotation.Classes[0];
                    object obj = Activator.CreateInstance(classDescriptorClass, new object[] { thatClass });
                    result = (ClassDescriptor)obj;
                    result.fieldDescriptorClass = descriptorsAnnotation.Classes[1];
                }

                GlobalClassDescriptorsMap.Add(className, result);

                result.DeriveAndOrganizeFieldsRecursive(thatClass);
                result._isGetAndOrganizeComplete = true;
            }

            return result;
        }

        /// <summary>
        ///     Recursive method to resolve annotations in parameter class and its super classes.
        ///     This mehthod creates field descriptors and other optimized datastructures, which 
        ///     is used for marshalling and ummarshalling of runtime objects.
        /// </summary>
        /// <param name="thatClass">
        ///     The parameter class <c>Type</c> to resolve any defined annotations.
        /// </param>
        /// <param name="fieldDescriptorClass">
        ///     Used by recursive call from inside the function. Can be null if being called 
        ///     for the first time. 
        /// </param>
        public void DeriveAndOrganizeFieldsRecursive(Type thatClass)
        {        
            if (XmlTools.IsAnnotationPresent<SimplInherit>(thatClass))
            {
                Type[] superClassGenericArguments = thatClass.BaseType.GetGenericArguments();
                ClassDescriptor superClassDescriptor = GetClassDescriptor(thatClass.BaseType);
                ReferFieldDescriptorsFrom(superClassDescriptor, superClassGenericArguments);
            }

            FieldInfo[] fields =
                thatClass.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                    BindingFlags.DeclaredOnly);
            
            // Iterate through all fields for the type
            foreach (FieldInfo thatField in fields)
            {
                // We don't serialize static values in S.im.pl, continue on to the next field
                if ((thatField.IsStatic)) continue;

                
                Int16 fieldType = FieldTypes.UnsetType;

                if (XmlTools.IsScalar(thatField))
                {
                    fieldType = FieldTypes.Scalar;
                }
                else if (XmlTools.IsAnnotationPresent<SimplComposite>(thatField))
                {
                    fieldType = FieldTypes.CompositeElement;
                }
                else if (XmlTools.IsAnnotationPresent<SimplCollection>(thatField))
                {
                    fieldType = FieldTypes.CollectionElement;
                }
                else if (XmlTools.IsAnnotationPresent<SimplMap>(thatField))
                {
                    fieldType = FieldTypes.MapElement;
                }

                if (fieldType == FieldTypes.UnsetType)
                {
                    continue; //not a simpl serialization annotated field
                }

                FieldDescriptor fieldDescriptor = NewFieldDescriptor(thatField, fieldType, fieldDescriptorClass);

                //generics
                fieldDescriptor.GenericTypeVarsContextCD = this;
 
                if (fieldDescriptor.FdType == FieldTypes.Scalar)
                {
                    Hint xmlHint = fieldDescriptor.XmlHint;
                    switch (xmlHint)
                    {
                        case Hint.XmlAttribute:
                            _attributeFieldDescriptors.Add(fieldDescriptor);
                            break;
                        case Hint.XmlText:
                        case Hint.XmlTextCdata:
                            break;
                        case Hint.XmlLeaf:
                        case Hint.XmlLeafCdata:
                            _elementFieldDescriptors.Add(fieldDescriptor);
                            break;
                    }
                }
                else
                    _elementFieldDescriptors.Add(fieldDescriptor);

                if (XmlTools.IsCompositeAsScalarValue(thatField))
                {
                    _scalarValueFieldDescriptor = fieldDescriptor;
                }


                _fieldDescriptorsByFieldName.Add(thatField.Name, fieldDescriptor);

                if (fieldDescriptor.IsMarshallOnly)
                    continue;

                String fieldTagName = fieldDescriptor.TagName;
                if (fieldDescriptor.IsWrapped)
                {
                    FieldDescriptor wrapper = NewFieldDescriptor(fieldDescriptor, fieldTagName, fieldDescriptorClass);
                    MapTagToFdForTranslateFrom(fieldTagName, wrapper);
                }
                else if (!fieldDescriptor.IsPolymorphic)
                {
                    String tag = fieldDescriptor.IsCollection ? fieldDescriptor.CollectionOrMapTagName : fieldTagName;
                    MapTagToFdForTranslateFrom(tag, fieldDescriptor);

                    var otherTagsAttributes = XmlTools.GetAnnotation<SimplOtherTags>(thatField);
                    String[] otherTags = XmlTools.OtherTags(otherTagsAttributes);

                    if (otherTags != null)
                    {
                        foreach (String otherTag in otherTags)
                        {
                            MapTagToFdForTranslateFrom(otherTag, fieldDescriptor);
                        }
                    }
                }
                else
                {
                    MapTagClassDescriptors(fieldDescriptor);
                }
            }
        }

        private void ReferFieldDescriptorsFrom(ClassDescriptor superClassDescriptor, Type[] superClassGenericArguments)
        {
            InitDeclaredGenericTypeVarNames();

            Dictionary<FieldDescriptor, FieldDescriptor> bookkeeper = new Dictionary<FieldDescriptor, FieldDescriptor>();

            foreach (String key in superClassDescriptor.FieldDescriptorByFieldName.Keys)
            {
                _fieldDescriptorsByFieldName.Put(key,
                    PerhapsCloneGenericField(superClassDescriptor.FieldDescriptorByFieldName[key], bookkeeper));
            }

            foreach (String key in superClassDescriptor.AllFieldDescriptorsByTagNames.Keys)
            {
                _allFieldDescriptorsByTagNames.Put(key,
                    PerhapsCloneGenericField(superClassDescriptor.AllFieldDescriptorsByTagNames[key], bookkeeper));
            }

            foreach (FieldDescriptor fd in superClassDescriptor.ElementFieldDescriptors)
            {
                _elementFieldDescriptors.Add(PerhapsCloneGenericField(fd, bookkeeper));
            }

            foreach (FieldDescriptor fd in superClassDescriptor.AttributeFieldDescriptors)
            {
                _attributeFieldDescriptors.Add(PerhapsCloneGenericField(fd, bookkeeper));
            }

            if (superClassDescriptor.UnresolvedScopeAnnotationFDs != null)
            {
                foreach (FieldDescriptor fd in superClassDescriptor.UnresolvedScopeAnnotationFDs)
                {
                    RegisterUnresolvedScopeAnnotationFD(fd);
                }
            }
        }

        //generics
	    private void InitDeclaredGenericTypeVarNames()
	    {
		    if (declaredGenericTypeVarNames == null && _describedClass != null)
		    {
			    List<String> result = new List<String>();
			    Type[] typeParams = _describedClass.GetGenericArguments();
			    if (typeParams != null && typeParams.Length > 0)
			    {
				    foreach (Type typeParam in typeParams)
					    result.Add(typeParam.Name);
			    }
			    if (result.Count > 0)
				    declaredGenericTypeVarNames = result;
		    }
	    }

        private FieldDescriptor PerhapsCloneGenericField(FieldDescriptor fd, Dictionary<FieldDescriptor, FieldDescriptor> bookkeeper)
	    {
		    if (declaredGenericTypeVarNames == null || fd.Field == null)
			    return fd;
		
		    if (bookkeeper.ContainsKey(fd))
			    return bookkeeper[fd];

            FieldDescriptor result = fd;
		    Type genericType = fd.Field.FieldType;

		    if (IsTypeUsingGenericNames(genericType, declaredGenericTypeVarNames))
		    {
                result = fd.Clone();
			    result.SetGenericTypeVars(null);
			    result.GenericTypeVarsContextCD = this;
		    }
		    bookkeeper.Add(fd, result);
		    return result;
	    }

	    private Boolean IsTypeUsingGenericNames(Type genericType, List<String> names)
	    {
		    if (genericType != null)
		    {
			    if (genericType.IsGenericParameter)
			    {
				    if (names.Contains(genericType.Name) || genericType.GetGenericParameterConstraints().Length > 0 && IsTypeUsingGenericNames(genericType.GetGenericParameterConstraints()[0], names))
					    return true;
			    }
			    else if (genericType.IsGenericType)
			    {
				    Type[] args = genericType.GetGenericArguments();
				    foreach (Type arg in args)
					    if (IsTypeUsingGenericNames(arg, names))
						    return true;
			    }
		    }
		    return false;
	    }

        protected List<FieldDescriptor> UnresolvedScopeAnnotationFDs
        {
            get { return _unresolvedScopeAnnotationFDs; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public FieldDescriptor GetFieldDescriptorByTag(String tagName)
        {

            if (_unresolvedScopeAnnotationFDs != null)
                ResolveUnresolvedScopeAnnotationFDs();

            if (_unresolvedClassesAnnotationFDs != null)
                ResolveUnresolvedClassesAnnotationFDs();

            return (AllFieldDescriptorsByTagNames.ContainsKey(tagName)) ? AllFieldDescriptorsByTagNames[tagName] : null;
        }

        public DictionaryList<String, FieldDescriptor> FieldDescriptorByFieldName
        {
            get { return _fieldDescriptorsByFieldName; }
        }


        public FieldDescriptor GetFieldDescriptorByFieldName(String fieldName)
        {
            FieldDescriptor result;
            _fieldDescriptorsByFieldName.TryGetValue(fieldName, out result);

            return result;
        }

        

        public IEnumerable<FieldDescriptor> GetAllFields()
        {

            if (_unresolvedScopeAnnotationFDs != null)
            {
                ResolveUnresolvedScopeAnnotationFDs();
            }
            foreach (FieldDescriptor fd in AllFieldDescriptorsByTagNames.Values)
            {
                if (fd == null)
                    continue;
                yield return fd;
            }
            yield break;
        }

        private void ResolveUnresolvedScopeAnnotationFDs()
        {
            if (_unresolvedScopeAnnotationFDs != null)
            {
                for (int i = _unresolvedScopeAnnotationFDs.Count - 1; i >= 0; i--)
                {
                    FieldDescriptor fd = _unresolvedScopeAnnotationFDs[i];
                    _unresolvedScopeAnnotationFDs.RemoveAt(i);
                    fd.ResolveUnresolvedScopeAnnotation();
                    MapTagClassDescriptors(fd);
                }
            }
            _unresolvedScopeAnnotationFDs = null;
        }

        public void ResolveUnresolvedClassesAnnotationFDs()
        {
            if (_unresolvedClassesAnnotationFDs != null)
            {
                for (int i = _unresolvedClassesAnnotationFDs.Count - 1; i >= 0; i--)
                {
                    FieldDescriptor fd = _unresolvedClassesAnnotationFDs[i];
                    _unresolvedClassesAnnotationFDs.RemoveAt(i);
                    fd.ResolveUnresolvedClassesAnnotation();
                    this.MapTagClassDescriptors(fd);
                    //this.MapTagClassDescriptors(fd);
                }
            }
        }

        /// <summary>
        ///     Method to map tags to their fieldDescriptors in the 
        ///     global mapping. 
        ///     This optimized datastructure is mainly used for translating from XML.
        /// </summary>
        /// <param name="tagName"><c>String</c> tagName of the FieldDescriptor</param>
        /// <param name="fdToMap"><c>FieldDescriptor</c> fdToMap to be added to the dictionary</param>
        private void MapTagToFdForTranslateFrom(String tagName, FieldDescriptor fdToMap)
        {
            if (!fdToMap.IsWrapped)
            {
                FieldDescriptor previousMapping = _allFieldDescriptorsByTagNames.Put(tagName, fdToMap);
                if (previousMapping != null && previousMapping != fdToMap)
                    Debug.WriteLine(" tag <" + tagName + ">:\tfield[" + fdToMap.Name + "] overrides field[" +
                                    previousMapping.Name + "]");
            }
        }

        private FieldDescriptor NewFieldDescriptor(FieldInfo thatField, Int16 annotationType, Type fieldDescriptorClass)
        {
            if (fieldDescriptorClass == null)
                return new FieldDescriptor(this, thatField, annotationType);

            Object[] args = new Object[3];
            args[0] = this;
            args[1] = thatField;
            args[2] = annotationType;
            if (annotationType == 6)
                annotationType += 0;

            return (FieldDescriptor) Activator.CreateInstance(fieldDescriptorClass, args);
        }

        protected FieldDescriptor NewFieldDescriptor(FieldDescriptor wrappedFD, String wrapperTag,
                                                     Type fieldDescriptorClass)
        {
            if (fieldDescriptorClass == null)
                return new FieldDescriptor(this, wrappedFD, wrapperTag);

            Object[] args = new Object[3];
            args[0] = this;
            args[1] = wrappedFD;
            args[2] = wrapperTag;

            return (FieldDescriptor) Activator.CreateInstance(fieldDescriptorClass, args);
        }

        /// <summary>
        ///     Gets the <c>FieldDescriptors</c> for fields represeted as
        ///     attributes in XML.
        /// </summary>
        public List<FieldDescriptor> AttributeFieldDescriptors
        {
            get { return _attributeFieldDescriptors; }
        }

        /// <summary>
        ///     Gets the <c>FieldDescriptors</c> for fields represented as 
        ///     leafs in XML
        /// </summary>
        public List<FieldDescriptor> ElementFieldDescriptors
        {
            get { return _elementFieldDescriptors; }
        }

        public IEnumerable<FieldDescriptor> AllFieldDescriptors
        {
            get
            {
                foreach (FieldDescriptor fd in _attributeFieldDescriptors)
                {
                    yield return fd;
                }

                foreach (FieldDescriptor fd in _elementFieldDescriptors)
                {
                    yield return fd;
                }
            }
        }

        /// <summary>
        ///     Gets the type of the class described by this <c>ClassDescriptor</c>
        /// </summary>
        public Type DescribedClass
        {
            get { return _describedClass; }
        }

        /// <summary>
        ///     Gets the tagName of the class
        /// </summary>
        public string TagName
        {
            get { return _tagName; }
        }

       
        /// <summary>
        ///     Gets the Pseudo FieldDescriptor
        /// </summary>
        public FieldDescriptor PseudoFieldDescriptor
        {
            get
            {
                FieldDescriptor result = _pseudoFieldDescriptor;
                if (result == null)
                {
                    result = new FieldDescriptor(this);
                    _pseudoFieldDescriptor = result;
                }
                return result;
            }
        }

        /// <summary>
        ///     Gets the <c>String</c> simple name of the described class.
        /// </summary>
        public string DescribedClassSimpleName
        {
            get { return _describedClassSimpleName; }
        }

        /// <summary>
        ///     Creates and returns the instance of the class described
        ///     by this <c>ClassDescriptor</c>
        /// </summary>
        public Object Instance
        {
            get { return XmlTools.GetInstance(_describedClass); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasScalarTextField
        {
            get { return _scalarTextFD != null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldDescriptor ScalarTextFD
        {
            get { return _scalarTextFD; }
            set { _scalarTextFD = value; }

        }

        /// <summary>
        /// 
        /// </summary>
        public FieldDescriptor ScalarValueFieldDescriptor
        {
            get { return _scalarValueFieldDescriptor; }
            set { _scalarValueFieldDescriptor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldDescriptor"></param>
        public void MapTagClassDescriptors(FieldDescriptor fieldDescriptor)
        {
            DictionaryList<String, ClassDescriptor> tagClassDescriptors = fieldDescriptor.PolymorphClassDescriptors;

            if (tagClassDescriptors != null)
                foreach (String tagName in tagClassDescriptors.Keys)
                {
                    MapTagToFdForTranslateFrom(tagName, fieldDescriptor);
                }
            MapTagToFdForTranslateFrom(fieldDescriptor.TagName, fieldDescriptor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        public void RegisterUnresolvedScopeAnnotationFD(FieldDescriptor fd)
        {
            if (_unresolvedScopeAnnotationFDs == null)
            {
                lock (this)
                {
                    if (_unresolvedScopeAnnotationFDs == null)
                        _unresolvedScopeAnnotationFDs = new List<FieldDescriptor>();
                }
            }
            _unresolvedScopeAnnotationFDs.Add(fd);
        }

        public static ClassDescriptor GetClassDescriptor(object obj)
        {
            return GetClassDescriptor(obj.GetType());
        }

        public override string JavaTypeName
        {
            get { return DescribedClassName; }
        }

        public override string CSharpTypeName
        {
            get { return DescribedClassName; }
        }

        public override string ObjectiveCTypeName
        {
            get { return DescribedClassName; }
        }

        public override string DbTypeName
        {
            get { return DescribedClassName; }
        }

        public String DescribedClassName
        {
            get
            {
                return DescribedClass != null
                           ? (!DescribedClass.IsGenericType 
                                ? DescribedClass.Name
                                //generics
                                : DescribedClass.Name.Substring(0,DescribedClass.Name.IndexOf('`')))
                           : _describedClassPackageName + "." + DescribedClassSimpleName;
            }
        }
       
        //generics
	    private void AddGenericTypeVariables()
	    {
            if (_describedClass.IsGenericType)
            {
                Type[] typeArguments = _describedClass.GetGenericArguments();

                foreach (Type tParam in typeArguments)
                {
                    if (tParam.IsGenericParameter)//true only for generic type parameters
                    {
                        String typeClassName = tParam.Name;
				        genericTypeVariables.Add(typeClassName);
                    }
                }
             }
	    }

        /// <summary>
        /// lazy-evaluation method. 
        /// </summary>
	    public List<GenericTypeVar> GetGenericTypeVars()
	    {
		    if (genericTypeVars == null)
		    {
                genericTypeVars = new List<GenericTypeVar>();
			    DeriveGenericTypeVariables();
		    }

		    return genericTypeVars;
	    }

        /// <summary>
        /// lazy-evaluation method. 
        /// </summary>
        public List<GenericTypeVar> GetSuperClassGenericTypeVars()
	    {
		    if (superClassGenericTypeVars == null)
		    {
			    deriveSuperGenericTypeVariables();
		    }

		    return superClassGenericTypeVars;
	    }

	    // added a setter to enable environment specific implementation -Fei
	    public List<GenericTypeVar> SuperClassGenericTypeVars
	    {
            set { superClassGenericTypeVars = value; }
	    }
	
	    // This method is modified, refer to FundamentalPlatformSpecific package -Fei
	    private void deriveSuperGenericTypeVariables()
	    {
		    FundamentalPlatformSpecifics.Get().DeriveSuperClassGenericTypeVars(this);
	    }

	    private void DeriveGenericTypeVariables()
	    {
		    if (_describedClass != null) // for generated descriptors, describedClass == null
		    {
			    Type[] typeVariables = _describedClass.GetGenericArguments();
			    if (typeVariables != null && typeVariables.Length > 0)
			    {
				    foreach (Type typeVariable in typeVariables)
				    {
					    GenericTypeVar g = GenericTypeVar.GetGenericTypeVarDef(typeVariable, this.genericTypeVars);
					    this.genericTypeVars.Add(g);
				    }
			    }
		    }
	    }
       
 
        public string BibtexType
        {
            get { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, FieldDescriptor> AllFieldDescriptorsByTagNames
        {
            get { return _allFieldDescriptorsByTagNames; }
        }

        public FieldDescriptor ScalarValueFieldDescripotor
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        public override List<string> OtherTags
        {
            get { throw new NotImplementedException(); }
        }


        public object GetInstance()
        {
            Console.WriteLine(this.DescribedClass.Name);
            return XmlTools.GetInstance(_describedClass);
        }

        public FieldDescriptor GetFieldDescriptorByTlvId(int blockType)
        {
            throw new NotImplementedException();
        }

        #region Implementation of IEnumerable

        public IEnumerator<FieldDescriptor> GetEnumerator()
        {
            return this._fieldDescriptorsByFieldName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void Replace(FieldDescriptor oldFD, FieldDescriptor newFD)
        {
            if (oldFD != null)
                AllFieldDescriptorsByTagNames.Remove(oldFD.TagName);
            AllFieldDescriptorsByTagNames.Put(newFD.TagName, newFD);
            if (oldFD != null)
            {
                Replace(AttributeFieldDescriptors, oldFD, newFD);
                Replace(ElementFieldDescriptors, oldFD, newFD);
            }
        }

        private static void Replace(List<FieldDescriptor> list, FieldDescriptor oldVal, FieldDescriptor newVal)
        {
            if (list == null)
                return;
            int i = list.IndexOf(oldVal);
            if (i > 0 && i < list.Count)
            {
                list[i] = newVal;
            }
        }
    }
}
