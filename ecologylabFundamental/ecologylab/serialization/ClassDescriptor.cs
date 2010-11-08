using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
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
        private DictionaryList<String, FieldDescriptor>
            fieldDescriptorsByFieldName = new DictionaryList<string, FieldDescriptor>();

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

        /// <summary>
        ///     Static dictionary containing all the <c>ClassDescriptors</c>, with their 
        ///     tagNames. This is used for fast access to class descriptors.
        /// </summary>
        private static Dictionary<String, ClassDescriptor>
            globalClassDescriptorsMap = new Dictionary<String, ClassDescriptor>();

        private List<FieldDescriptor> unresolvedScopeAnnotationFDs = null;


        #endregion

        #region Constructors

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
            this.tagName = XMLTools.GetXmlTagName(thatClass, TranslationScope.STATE);
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
                    result = new ClassDescriptor(thatClass);
                    globalClassDescriptorsMap.Add(className, result);
                    result.DeriveAndOrganizeFieldsRecursive(thatClass, null);
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
            if (XMLTools.IsAnnotationPresent(thatClass, typeof(simpl_inherit)))
            {
                Type superClass = thatClass.BaseType;

                if (superClass != null)
                    DeriveAndOrganizeFieldsRecursive(superClass, fieldDescriptorClass);
            }

            FieldInfo[] fields = thatClass.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo thatField in fields)
            {
                if ((thatField.IsStatic)) continue;

                Int16 fieldType = UNSET_TYPE;

                if (XMLTools.IsScalar(thatField))
                {
                    fieldType = SCALAR;
                }
                else if (XMLTools.IsAnnotationPresent(thatField, typeof(simpl_composite)))
                {
                    fieldType = COMPOSITE_ELEMENT;
                }
                else if (XMLTools.IsAnnotationPresent(thatField, typeof(simpl_collection)))
                {
                    fieldType = COLLECTION_ELEMENT;
                }
                else if (XMLTools.IsAnnotationPresent(thatField, typeof(simpl_map)))
                {
                    fieldType = MAP_ELEMENT;
                }

                if (fieldType == UNSET_TYPE)
                    continue; //not a simpl serialization annotated field

                FieldDescriptor fieldDescriptor = new FieldDescriptor(this, thatField, fieldType);

                if (fieldDescriptor.Type == SCALAR)
                {
                    Hint xmlHint = fieldDescriptor.XmlHint;
                    switch (xmlHint)
                    {
                        case Hint.XML_ATTRIBUTE:
                            attributeFieldDescriptors.Add(fieldDescriptor);
                            break;
                        case Hint.XML_TEXT:
                        case Hint.XML_TEXT_CDATA:
                            break;
                        case Hint.XML_LEAF:
                        case Hint.XML_LEAF_CDATA:
                            elementFieldDescriptors.Add(fieldDescriptor);
                            break;
                    }
                }
                else
                    elementFieldDescriptors.Add(fieldDescriptor);

                if (fieldDescriptor.IsMarshallOnly)
                    continue;

                String fieldTagName = fieldDescriptor.TagName;
                if (fieldDescriptor.IsWrapped)
                {
                    FieldDescriptor wrapper = new FieldDescriptor(this, fieldDescriptor, fieldTagName);
                    MapTagToFdForTranslateFrom(fieldTagName, wrapper);
                }
                else if (!fieldDescriptor.IsPolymorphic)
                {
                    String tag = fieldDescriptor.IsCollection ? fieldDescriptor.CollectionOrMapTagName : fieldTagName;
                    MapTagToFdForTranslateFrom(tag, fieldDescriptor);

                    xml_other_tags otherTagsAttributes = (xml_other_tags)XMLTools.GetAnnotation(thatField, typeof(xml_other_tags));
                    String[] otherTags = XMLTools.OtherTags(otherTagsAttributes);
                    if (otherTags != null && otherTags.Length > 0)
                    {
                        foreach (String otherTag in otherTags)
                            MapTagToFdForTranslateFrom(otherTag, fieldDescriptor);
                    }
                }
                else
                {
                    foreach (ClassDescriptor classDescriptor in fieldDescriptor.TagClassDescriptors.Values)
                    {
                        MapTagToFdForTranslateFrom(classDescriptor.tagName, fieldDescriptor);
                    }
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
                return XMLTools.GetInstance(describedClass);
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
