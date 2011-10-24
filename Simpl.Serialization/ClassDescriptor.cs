using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.Serializers;
using ecologylab.serialization;

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
    public class ClassDescriptor : DescriptorBase
    {
        /// <summary>
        ///     Holds the <c>Type</c> of the class described by this 
        ///     <c>ClassDescriptor</c>
        /// </summary>
        private readonly Type _describedClass;

        /// <summary>
        ///     Holds the <c>String</c> tagName of the class described by this
        ///     <c>ClassDescriptor</c>
        /// </summary>
        private readonly String _tagName;

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

        private FieldDescriptor _scalarValueFieldDescriptor;

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
        public ClassDescriptor(Type thatClass)
            : base(XmlTools.GetXmlTagName(thatClass, SimplTypesScope.STATE), thatClass.Name)
        {
            _describedClass = thatClass;
            _describedClassSimpleName = thatClass.Name;
            _describedClassPackageName = thatClass.Namespace;
            _tagName = XmlTools.GetXmlTagName(thatClass, SimplTypesScope.STATE);

            if (XmlTools.IsAnnotationPresent(thatClass, typeof (SimplUseEqualsEquals)))
            {
                _strictObjectGraphRequired = true;
            }

            SimplDescriptorClasses descriptorsClassesAttribute =
                (SimplDescriptorClasses) XmlTools.GetAnnotation(thatClass, typeof (SimplDescriptorClasses));
            if(descriptorsClassesAttribute != null)
            {
                classDescriptorClass = descriptorsClassesAttribute.Classes[0];
                fieldDescriptorClass = descriptorsClassesAttribute.Classes[1];
            }
        }

        /// <summary>
        ///     Constructor for initializing the 
        ///     <c>ClassDescriptor</c> with the <c>tag</c>
        /// </summary>
        /// <param name="tagName">
        ///     <c>String</c> tagName for the <c>ClassDescriptor</c>
        /// </param>
        public ClassDescriptor(String tagName, String comment, String describedClassPackageName,
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
            String className = thatClass.FullName;
            ClassDescriptor result;

            if (!GlobalClassDescriptorsMap.TryGetValue(className, out result))
            {
                var descriptorsAnnotation =
                    (SimplDescriptorClasses) XmlTools.GetAnnotation(thatClass, typeof (SimplDescriptorClasses), true);
                if (descriptorsAnnotation == null)
                    result = new ClassDescriptor(thatClass);
                else
                {
                    //First class is the type of the class descriptor, the second the type of the fieldDescriptor.
                    Type classDescriptorClass = descriptorsAnnotation.Classes[0];
                    object obj = Activator.CreateInstance(classDescriptorClass, new object[] {thatClass});
                    result = (ClassDescriptor) obj;
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
            if (XmlTools.IsAnnotationPresent(thatClass, typeof (SimplInherit)))
            {
                ClassDescriptor superClassDescriptor = GetClassDescriptor(thatClass.BaseType);
                ReferFieldDescriptorsFrom(superClassDescriptor);
            }

            FieldInfo[] fields =
                thatClass.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                    BindingFlags.DeclaredOnly);
            foreach (FieldInfo thatField in fields)
            {
                if ((thatField.IsStatic)) continue;

                Int16 fieldType = FieldTypes.UnsetType;

                if (XmlTools.IsScalar(thatField))
                {
                    fieldType = FieldTypes.Scalar;
                }
                else if (XmlTools.IsAnnotationPresent(thatField, typeof (SimplComposite)))
                {
                    fieldType = FieldTypes.CompositeElement;
                }
                else if (XmlTools.IsAnnotationPresent(thatField, typeof (SimplCollection)))
                {
                    fieldType = FieldTypes.CollectionElement;
                }
                else if (XmlTools.IsAnnotationPresent(thatField, typeof (SimplMap)))
                {
                    fieldType = FieldTypes.MapElement;
                }

                if (fieldType == FieldTypes.UnsetType)
                    continue; //not a simpl serialization annotated field

                FieldDescriptor fieldDescriptor = NewFieldDescriptor(thatField, fieldType, fieldDescriptorClass);

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

                    var otherTagsAttributes =
                        (SimplOtherTags) XmlTools.GetAnnotation(thatField, typeof (SimplOtherTags));
                    String[] otherTags = XmlTools.OtherTags(otherTagsAttributes);
                    if (otherTags != null && otherTags.Length > 0)
                    {
                        foreach (String otherTag in otherTags)
                            MapTagToFdForTranslateFrom(otherTag, fieldDescriptor);
                    }
                }
                else
                {
                    MapTagClassDescriptors(fieldDescriptor);
                }
            }
        }

        private void ReferFieldDescriptorsFrom(ClassDescriptor superClassDescriptor)
        {
            foreach (String key in superClassDescriptor.FieldDescriptorByFieldName.Keys)
            {
                _fieldDescriptorsByFieldName.Put(key, superClassDescriptor.FieldDescriptorByFieldName[key]);
            }

            foreach (String key in superClassDescriptor.AllFieldDescriptorsByTagNames.Keys)
            {
                _allFieldDescriptorsByTagNames.Put(key, superClassDescriptor.AllFieldDescriptorsByTagNames[key]);
            }

            foreach (FieldDescriptor fd in superClassDescriptor.ElementFieldDescriptors)
            {
                _elementFieldDescriptors.Add(fd);
            }

            foreach (FieldDescriptor fd in superClassDescriptor.AttributeFieldDescriptors)
            {
                _attributeFieldDescriptors.Add(fd);
            }

            if (superClassDescriptor.UnresolvedScopeAnnotationFDs != null)
            {
                foreach (FieldDescriptor fd in superClassDescriptor.UnresolvedScopeAnnotationFDs)
                {
                    RegisterUnresolvedScopeAnnotationFD(fd);
                }
            }
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
            {
                ResolveUnresolvedScopeAnnotationFDs();
            }
            if (AllFieldDescriptorsByTagNames.ContainsKey(tagName))
                return AllFieldDescriptorsByTagNames[tagName];
            return null;
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



        /// <summary>
        ///     Method to map tags to their fieldDescriptors in the 
        ///     global mapping. 
        ///     This optimized datastructure is mainly used for translating from XML.
        /// </summary>
        /// <param name="tagName"><c>String</c> tagName of the FieldDescriptor</param>
        /// <param name="fdToMap"><c>FieldDescriptor</c> fdToMap to be added to the dictionary</param>
        private void MapTagToFdForTranslateFrom(String tagName, FieldDescriptor fdToMap)
        {
            FieldDescriptor previousMapping;

            if (AllFieldDescriptorsByTagNames.TryGetValue(tagName, out previousMapping))
            {
                //Debug.WriteLine(" tag <" + tagName + ">:\tfield[" + fdToMap.FieldName + "] overrides field[" + previousMapping.FieldName + "]");
                AllFieldDescriptorsByTagNames.Remove(tagName);
            }

            AllFieldDescriptorsByTagNames.Add(tagName, fdToMap);

        }

        private FieldDescriptor NewFieldDescriptor(FieldInfo thatField, Int16 annotationType, Type fieldDescriptorClass)
        {
            if (fieldDescriptorClass == null)
                return new FieldDescriptor(this, thatField, annotationType);

            Object[] args = new Object[3];
            args[0] = this;
            args[1] = thatField;
            args[2] = annotationType;

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
                    yield return fd;
                foreach (FieldDescriptor fd in _elementFieldDescriptors)
                    yield return fd;
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

        public override List<string> OtherTags
        {
            get { throw new NotImplementedException(); }
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
                           ? DescribedClass.Name
                           : _describedClassPackageName + "." + DescribedClassSimpleName;
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


        public object GetInstance()
        {
            return XmlTools.GetInstance(_describedClass);
        }
    }
}
