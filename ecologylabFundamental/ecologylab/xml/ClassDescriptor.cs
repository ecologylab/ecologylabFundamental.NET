using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ecologylabFundamental.ecologylab.generic;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml
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
    ///     <para>
    ///     <author>Nabeel Shahzad (Interface Ecology Lab)</author>
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

        /// <summary>
        ///     TODO: why we need this? remove if not required.
        /// </summary>
        private ElementState thatClass;

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
        /// <param name="tag">
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
            if (XMLTools.IsAnnotationPresent(thatClass, typeof(xml_inherit)))
            {
                Type superClass = thatClass.BaseType;

                if (fieldDescriptorClass == null)
                {
                    fieldDescriptorClass = FieldDescriptorAnnotationValue(thatClass);
                }

                if (superClass != null)
                    DeriveAndOrganizeFieldsRecursive(superClass, fieldDescriptorClass);
            }

            FieldInfo[] fields = thatClass.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo thatField in fields)
            {
                if ((thatField.IsStatic)) continue;

                FieldDescriptor fieldDescriptor = null;
                Boolean isElement = true;
                if (XMLTools.IsAnnotationPresent(thatField, typeof(xml_attribute)))
                {
                    isElement = false;
                    fieldDescriptor = new FieldDescriptor(this, thatField, ATTRIBUTE);
                }
                else if (XMLTools.IsAnnotationPresent(thatField, typeof(xml_leaf)))
                {
                    fieldDescriptor = new FieldDescriptor(this, thatField, LEAF);
                }
                else if (XMLTools.IsAnnotationPresent(thatField, typeof(xml_text)))
                {
                    fieldDescriptor = new FieldDescriptor(this, thatField, TEXT_ELEMENT);
                }
                else if (XMLTools.IsAnnotationPresent(thatField, typeof(xml_nested)))
                {
                    fieldDescriptor = new FieldDescriptor(this, thatField, NESTED_ELEMENT);
                }
                else if (XMLTools.IsAnnotationPresent(thatField, typeof(xml_collection)))
                {
                    fieldDescriptor = new FieldDescriptor(this, thatField, COLLECTION_ELEMENT);
                }
                else if (XMLTools.IsAnnotationPresent(thatField, typeof(xml_map)))
                {
                    fieldDescriptor = new FieldDescriptor(this, thatField, MAP_ELEMENT);
                }
                else
                    continue;

                if (isElement)
                    elementFieldDescriptors.Add(fieldDescriptor);
                else
                    attributeFieldDescriptors.Add(fieldDescriptor);

                fieldDescriptorsByFieldName.Add(thatField.Name, fieldDescriptor);

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
                    foreach (ClassDescriptor classDescriptor in fieldDescriptor.TagClassDescriptors)
                    {
                        MapTagToFdForTranslateFrom(classDescriptor.tagName, fieldDescriptor);
                    }
                }
            }
        }

        /// <summary>
        ///     Public method to get the field descriptor from its tagName.
        ///     TODO: why this function takes all these parameters. Remove them!
        /// </summary>
        /// <param name="tagName">
        ///     <c>String</c> tagName for to find the associated <c>FieldDescriptor</c>
        /// </param>
        /// <param name="translationScope"></param>
        /// <param name="currentElementState"></param>
        /// <returns></returns>
        public FieldDescriptor GetFieldDescriptorByTag(String tagName, TranslationScope translationScope, ElementState currentElementState)
        {
            if (allFieldDescriptorsByTagNames.ContainsKey(tagName))
                return allFieldDescriptorsByTagNames[tagName];
            else
                return null;
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
                Console.WriteLine(" tag <" + tagName + ">:\tfield[" + fdToMap.FieldName + "] overrides field[" + previousMapping.FieldName + "]");

            }

            allFieldDescriptorsByTagNames.Add(tagName, fdToMap);
        }
        
        /// <summary>
        ///     TODO: implement or remove this functions :) 
        /// </summary>
        /// <param name="thatClass"></param>
        /// <returns></returns>
        private Type FieldDescriptorAnnotationValue(Type thatClass)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the <c>FieldDescriptors</c> for fields represeted as
        ///      attributes in XML.
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

        #endregion
    }
}
