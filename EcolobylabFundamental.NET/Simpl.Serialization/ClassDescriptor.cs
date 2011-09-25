using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Simpl.Serialization;
using Simpl.Serialization.Attributes;
using ecologylab.generic;
using ecologylab.attributes;

namespace ecologylab.serialization
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
    public class ClassDescriptor : FieldTypes
    {
        #region Private Fields

        /// <summary>
        ///     Holds the <c>Type</c> of the class described by this 
        ///     <c>ClassDescriptor</c>
        /// </summary>
        private Type describedClass;

        /// <summary>
        ///     Holds the <c>String</c> tagName of the class described by this
        ///     <c>ClassDescriptor</c>
        /// </summary>
        private String tagName;

        /// <summary>
        ///     Holds the <c>String</c> simple name of the described class.
        /// </summary>
        private String describedClassSimpleName;

        /// <summary>
        ///     Holds the <c>String</c> package name of the described class.
        ///     This comes from Java version. 
        ///     TODO: see if this can be removed or add namespace name here.
        /// </summary>
        private String describedClassPackageName;

        /// <summary>
        ///     An abstract <c>FieldDescriptor</c> for wrapped collections. This
        ///     only holds the tag name for the field since there is no field 
        ///     associated with the <c>FieldDescriptor</c>.
        /// </summary>
        private FieldDescriptor pseudoFieldDescriptor;

        /// <summary>
        ///     Not sure if this being used. Comes from Java version.
        ///     TODO: figure out and remove if necessary
        /// </summary>
        private FieldDescriptor scalarTextFD;

        /// <summary>
        ///     This flag indicates if the framework has completed resolving  
        ///     annotations for the class described by this <c>ClassDescriptor</c> 
        ///     or its super classes.
        /// </summary>
        private Boolean isGetAndOrganizeComplete;

        /// <summary>
        ///     This dictionary holds the association of field descriptors to
        ///     field names.
        ///     Does this contains all field descriptors for this class descriptor??.
        /// </summary>
        //private DictionaryList<String, FieldDescriptor>
        //    fieldDescriptorsByFieldName = new DictionaryList<string, FieldDescriptor>();

        /// <summary>
        ///     This dictionary holds the association of field descriptors to
        ///     tagNames. Should contain all field descriptors associated with this
        ///     <c>ClassDescriptor</c>.
        /// </summary>
        private Dictionary<String, FieldDescriptor>
            allFieldDescriptorsByTagNames = new Dictionary<String, FieldDescriptor>();

        /// <summary>
        ///     List of <c>FieldDescriptors</c>, which are represented as attributes 
        ///     when marshalled.
        /// </summary>
        private List<FieldDescriptor> attributeFieldDescriptors = new List<FieldDescriptor>();

        /// <summary>
        ///     List of <c>FieldDescriptors</c>, which are represented as leaf nodes 
        ///     when marshalled.
        /// </summary>
        private List<FieldDescriptor> elementFieldDescriptors = new List<FieldDescriptor>();

        /**
	     * Map of FieldToXMLOptimizations, with field names as keys.
	     * 
	     * Used to optimize translateToXML(). Also handy for providing functionality like associative
	     * arrays in Perl, JavaScript, PHP, ..., but with less overhead, because the hashtable is only
	     * maintained per class, not per instance.
	     */
	    [SimplNowrap]
	    [SimplMap("field_descriptor")]
        private DictionaryList<String, FieldDescriptor> _fieldDescriptorsByFieldName = new DictionaryList<String, FieldDescriptor>();

        /// <summary>
        ///     Static dictionary containing all the <c>ClassDescriptors</c>, with their 
        ///     tagNames. This is used for fast access to class descriptors.
        /// </summary>
        private static Dictionary<String, ClassDescriptor>
            globalClassDescriptorsMap = new Dictionary<String, ClassDescriptor>();

        private List<FieldDescriptor> unresolvedScopeAnnotationFDs = null;

        private FieldDescriptor scalarValueFieldDescriptor = null;
        
        /// <summary>
        /// defines whether a strict object graph is required based on the equality operator
        /// </summary>
        [SimplScalar]
        private bool strictObjectGraphRequired = false;

        /// <summary>
        /// Whether a strict object graph is required
        /// </summary>
        public bool StrictObjectGraphRequired
        {
            get { return strictObjectGraphRequired; }
            set { strictObjectGraphRequired = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Empty constructor for base classes
        /// </summary>
        public ClassDescriptor()
        {

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
        {
            this.describedClass = thatClass;
            this.describedClassSimpleName = thatClass.Name;
            this.describedClassPackageName = thatClass.Namespace;
            this.tagName = XmlTools.GetXmlTagName(thatClass, TranslationScope.STATE);

            if(XmlTools.IsAnnotationPresent(thatClass,typeof(SimplUseEqualsEquals)))
            {
                this.strictObjectGraphRequired = true;
            }
        }

        /// <summary>
        ///     Constructor for initializing the 
        ///     <c>ClassDescriptor</c> with the <c>tag</c>
        /// </summary>
        /// <param name="tagName">
        ///     <c>String</c> tagName for the <c>ClassDescriptor</c>
        /// </param>
        public ClassDescriptor(String tagName)
        {
            this.tagName = tagName;
        }

        #endregion

        #region Static Methods

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
            String className = thatClass.Name;
            ClassDescriptor result = null;

            if (result == null || !result.isGetAndOrganizeComplete)
            {
                if (!globalClassDescriptorsMap.TryGetValue(className, out result))
                {
                    SimplDescriptorClasses descriptorsAnnotation = (SimplDescriptorClasses)XmlTools.GetAnnotation(thatClass, typeof(SimplDescriptorClasses));
                    Type fieldDesctriptorType = null;
                    if (descriptorsAnnotation == null) 
                        result = new ClassDescriptor(thatClass);
                    else
                    {
                        //First class is the type of the class descriptor, the second the type of the fieldDescriptor.
                        Type classDescriptorClass   = descriptorsAnnotation.Classes[0];
                        fieldDesctriptorType = descriptorsAnnotation.Classes[1];
                        object obj = Activator.CreateInstance(classDescriptorClass, new object[] { thatClass });;
                        result = (ClassDescriptor)obj;
                    }
                    globalClassDescriptorsMap.Add(className, result);
                    
                    result.DeriveAndOrganizeFieldsRecursive(thatClass, fieldDesctriptorType);
                    result.isGetAndOrganizeComplete = true;
                }
            }

            return result;
        }

        /// <summary>
        ///     Returns the <c>ClassDescriptor</c> associated with the type of parameter class
        ///     Uses the global class descriptor map to fetch the <c>ClassDescriptor</c>.
        ///     If it is being for the first time it recusively generate <c>ClassDescriptors</c>
        /// </summary>
        /// <param name="elementState"></param>
        /// <returns>
        ///     <c>ClassDescripor</c> for the paramater class type or <c>null</c> 
        ///     if there is no associated class descriptor.
        /// </returns>
        public static ClassDescriptor GetClassDescriptor(ElementState elementState)
        {
            return GetClassDescriptor(elementState.GetType());
        }

        #endregion

        #region Public Methods

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
        public void DeriveAndOrganizeFieldsRecursive(Type thatClass, Type fieldDescriptorClass)
        {
            if (XmlTools.IsAnnotationPresent(thatClass, typeof(SimplInherit)))
            {
                Type superClass = thatClass.BaseType;

                if (superClass != null)
                    DeriveAndOrganizeFieldsRecursive(superClass, fieldDescriptorClass);

            }

            FieldInfo[] fields = thatClass.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (FieldInfo thatField in fields)
            {
                if ((thatField.IsStatic)) continue;

                Int16 fieldType = UNSET_TYPE;

                if (XmlTools.IsScalar(thatField))
                {
                    fieldType = SCALAR;
                }
                else if (XmlTools.IsAnnotationPresent(thatField, typeof(SimplComposite)))
                {
                    fieldType = COMPOSITE_ELEMENT;
                }
                else if (XmlTools.IsAnnotationPresent(thatField, typeof(SimplCollection)))
                {
                    fieldType = COLLECTION_ELEMENT;
                }
                else if (XmlTools.IsAnnotationPresent(thatField, typeof(SimplMap)))
                {
                    fieldType = MAP_ELEMENT;
                }

                if (fieldType == UNSET_TYPE)
                    continue; //not a simpl serialization annotated field

                FieldDescriptor fieldDescriptor = NewFieldDescriptor(thatField, fieldType, fieldDescriptorClass);

                if (fieldDescriptor.Type == SCALAR)
                {
                    Hint xmlHint = fieldDescriptor.XmlHint;
                    switch (xmlHint)
                    {
                        case Hint.XmlAttribute:
                            attributeFieldDescriptors.Add(fieldDescriptor);
                            break;
                        case Hint.XmlText:
                        case Hint.XmlTextCdata:
                            break;
                        case Hint.XmlLeaf:
                        case Hint.XmlLeafCdata:
                            elementFieldDescriptors.Add(fieldDescriptor);
                            break;
                    }
                }
                else
                    elementFieldDescriptors.Add(fieldDescriptor);

                if (XmlTools.IsCompositeAsScalarValue(thatField))
                {
                    scalarValueFieldDescriptor = fieldDescriptor;
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

                    SimplOtherTags otherTagsAttributes = (SimplOtherTags)XmlTools.GetAnnotation(thatField, typeof(SimplOtherTags));
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

        /// <summary>
        ///     Public method to get the field descriptor from its tagName.
        /// <param name="tagName">
        ///     <c>String</c> tagName for to find the associated <c>FieldDescriptor</c>
        /// </param>
        /// <returns></returns>
        public FieldDescriptor GetFieldDescriptorByTag(String tagName)
        {

            if (unresolvedScopeAnnotationFDs != null)
            {
                ResolveUnresolvedScopeAnnotationFDs();
            }
            if (allFieldDescriptorsByTagNames.ContainsKey(tagName))
                return allFieldDescriptorsByTagNames[tagName];
            else
                return null;
        }

        /// <summary>
        ///     Public method to get the field descriptor from its field name.
        /// <param name="tagName">
        ///     <c>String</c> tagName for to find the associated <c>FieldDescriptor</c>
        /// </param>
        /// <returns></returns>
        public FieldDescriptor getFieldDescriptorByFieldName(String fieldName)
        {
            FieldDescriptor result = null;
            _fieldDescriptorsByFieldName.TryGetValue(fieldName, out result);

            return result;
        }

        /// <summary>
        /// Iterator through all these fields
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FieldDescriptor> GetAllFields()
        {

            if (unresolvedScopeAnnotationFDs != null)
            {
                ResolveUnresolvedScopeAnnotationFDs();
            }
            foreach (FieldDescriptor fd in allFieldDescriptorsByTagNames.Values)
            {
                if (fd == null)
                    continue;
                yield return fd;
            }
            yield break;
        }

        private void ResolveUnresolvedScopeAnnotationFDs()
        {
            if (unresolvedScopeAnnotationFDs != null)
            {
                for (int i = unresolvedScopeAnnotationFDs.Count - 1; i >= 0; i--)
                {
                    // TODO -- do we want to enable retrying multiple times in case it gets fixed even later
                    FieldDescriptor fd = unresolvedScopeAnnotationFDs[i];
                    unresolvedScopeAnnotationFDs.RemoveAt(i);
                    fd.ResolveUnresolvedScopeAnnotation();
                }
            }
            unresolvedScopeAnnotationFDs = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Method to map tags to their fieldDescriptors in the 
        ///     global mapping. 
        ///     This optimized datastructure is mainly used for translating from XML.
        /// </summary>
        /// <param name="tagName"><c>String</c> tagName of the FieldDescriptor</param>
        /// <param name="fdToMap"><c>FieldDescriptor</c> fdToMap to be added to the dictionary</param>
        private void MapTagToFdForTranslateFrom(String tagName, FieldDescriptor fdToMap)
        {
            FieldDescriptor previousMapping = null;

            if (allFieldDescriptorsByTagNames.TryGetValue(tagName, out previousMapping))
            {
                //Console.WriteLine(" tag <" + tagName + ">:\tfield[" + fdToMap.FieldName + "] overrides field[" + previousMapping.FieldName + "]");
                allFieldDescriptorsByTagNames.Remove(tagName);
            }

            allFieldDescriptorsByTagNames.Add(tagName, fdToMap);
            
        }

        private FieldDescriptor NewFieldDescriptor(FieldInfo thatField, Int16 annotationType, Type fieldDescriptorClass)
	    {
		    if (fieldDescriptorClass == null)
			    return new FieldDescriptor(this, thatField, annotationType);

		    Object[] args = new Object[3];
		    args[0] = this;
		    args[1] = thatField;
		    args[2] = annotationType;

            return (FieldDescriptor)Activator.CreateInstance(fieldDescriptorClass, args);
	    }

        protected FieldDescriptor NewFieldDescriptor(FieldDescriptor wrappedFD, String wrapperTag, Type fieldDescriptorClass)
        {
            if (fieldDescriptorClass == null)
			return new FieldDescriptor(this, wrappedFD, wrapperTag);

		    Object[] args = new Object[3];
		    args[0] = this;
		    args[1] = wrappedFD;
		    args[2] = wrapperTag;

            return (FieldDescriptor) Activator.CreateInstance(fieldDescriptorClass, args);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the <c>FieldDescriptors</c> for fields represeted as
        ///     attributes in XML.
        /// </summary>
        public List<FieldDescriptor> AttributeFieldDescriptors
        {
            get
            {
                return attributeFieldDescriptors;
            }
        }

        /// <summary>
        ///     Gets the <c>FieldDescriptors</c> for fields represented as 
        ///     leafs in XML
        /// </summary>
        public List<FieldDescriptor> ElementFieldDescriptors
        {
            get
            {
                return elementFieldDescriptors;
            }
        }

        public IEnumerable<FieldDescriptor> AllFieldDescriptors
        {
            get
            {
                foreach (FieldDescriptor fd in attributeFieldDescriptors)
                    yield return fd;
                foreach (FieldDescriptor fd in elementFieldDescriptors)
                    yield return fd;
            }
        }
        /// <summary>
        ///     Gets the type of the class described by this <c>ClassDescriptor</c>
        /// </summary>
        public Type DescribedClass
        {
            get
            {
                return describedClass;
            }
        }

        /// <summary>
        ///     Gets the tagName of the class
        /// </summary>
        public string TagName
        {
            get
            {
                return tagName;
            }
        }

        /// <summary>
        ///     Gets the Pseudo FieldDescriptor
        /// </summary>
        public FieldDescriptor PseudoFieldDescriptor
        {
            get
            {
                FieldDescriptor result = pseudoFieldDescriptor;
                if (result == null)
                {
                    result = new FieldDescriptor(this);
                    pseudoFieldDescriptor = result;
                }
                return result;
            }
        }

        /// <summary>
        ///     Gets the <c>String</c> simple name of the described class.
        /// </summary>
        public string DescribedClassSimpleName
        {
            get
            {
                return describedClassSimpleName;
            }
        }

        /// <summary>
        ///     Creates and returns the instance of the class described
        ///     by this <c>ClassDescriptor</c>
        /// </summary>
        public ElementState Instance
        {
            get
            {
                return XmlTools.GetInstance(describedClass);
            }
        }

        /// <summary>
        ///     Don't know what this is :| 
        ///     comes from the java version
        ///     TODO: remove if not required.
        /// </summary>
        public bool HasScalarTextField
        {
            get
            {
                return scalarTextFD != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldDescriptor ScalarTextFD
        {
            get
            {
                return scalarTextFD;
            }
            set
            {
                scalarTextFD = value;
            }
        
        }

        public FieldDescriptor ScalarValueFieldDescriptor
        {
            get { return scalarValueFieldDescriptor; }
            set { scalarValueFieldDescriptor = value; }
        }

        #endregion       
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldDescriptor"></param>
        public void MapTagClassDescriptors(FieldDescriptor fieldDescriptor)
        {
            DictionaryList<String, ClassDescriptor> tagClassDescriptors = fieldDescriptor.TagClassDescriptors;

            if (tagClassDescriptors != null)
                foreach (String tagName in tagClassDescriptors.Keys)
                {
                    MapTagToFdForTranslateFrom(tagName, fieldDescriptor);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        public void RegisterUnresolvedScopeAnnotationFD(FieldDescriptor fd)
        {
            if (unresolvedScopeAnnotationFDs == null)
            {
                lock (this)
                {
                    if (unresolvedScopeAnnotationFDs == null)
                        unresolvedScopeAnnotationFDs = new List<FieldDescriptor>();
                }
            }
            unresolvedScopeAnnotationFDs.Add(fd);
        }

    }
}
