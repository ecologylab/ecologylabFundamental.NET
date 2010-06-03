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
    /// 
    /// </summary>
    public class ClassDescriptor : FieldTypes
    {
        #region Private Fields

        private Type describedClass;
        private String tagName;
        private String describedClassSimpleName;
        private String describedClassPackageName;

        private FieldDescriptor pseudoFieldDescriptor;
        private FieldDescriptor scalarTextFD;
        private Boolean isGetAndOrganizeComplete;

        private DictionaryList<String, FieldDescriptor>
            fieldDescriptorsByFieldName = new DictionaryList<string, FieldDescriptor>();

        private Dictionary<String, FieldDescriptor>
            allFieldDescriptorsByTagNames = new Dictionary<String, FieldDescriptor>();

        private List<FieldDescriptor> attributeFieldDescriptors = new List<FieldDescriptor>();
        private List<FieldDescriptor> elementFieldDescriptors = new List<FieldDescriptor>();

        private static Dictionary<String, ClassDescriptor>
            globalClassDescriptorsMap = new Dictionary<String, ClassDescriptor>();

        private ElementState thatClass;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        public ClassDescriptor(Type thatClass)
        {
            this.describedClass = thatClass;
            this.describedClassSimpleName = thatClass.Name;
            this.describedClassPackageName = thatClass.Namespace;
            this.tagName = XMLTools.GetXmlTagName(thatClass, TranslationScope.STATE);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public ClassDescriptor(String tag)
        {
            this.tagName = tag;
        }

        #endregion

        #region Static Methods

        /// <summary>
        ///     Returns the <c>ClassDescriptor</c> 
        ///     associated with the class type.
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
        /// 
        /// </summary>
        /// <param name="elementState"></param>
        /// <returns></returns>
        public static ClassDescriptor GetClassDescriptor(ElementState elementState)
        {
            Type thatClass = elementState.GetType();
            return GetClassDescriptor(thatClass);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass">Testing </param>
        /// <param name="fieldDescriptorClass">testing</param>
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
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="translationScope"></param>
        /// <param name="currentElementState"></param>
        /// <returns></returns>
        public FieldDescriptor GetFieldDescriptorByTag(string tagName, TranslationScope translationScope, ElementState currentElementState)
        {
            if (allFieldDescriptorsByTagNames.ContainsKey(tagName))
                return allFieldDescriptorsByTagNames[tagName];
            else
                return null;
        }

        #endregion

        #region Private Methods

        private void MapTagToFdForTranslateFrom(String tagName, FieldDescriptor fdToMap)
        {
            FieldDescriptor previousMapping = null;

            if (allFieldDescriptorsByTagNames.TryGetValue(tagName, out previousMapping))
            {
                Console.WriteLine(" tag <" + tagName + ">:\tfield[" + fdToMap.FieldName + "] overrides field[" + previousMapping.FieldName + "]");

            }
            allFieldDescriptorsByTagNames.Add(tagName, fdToMap);
        }
        
        private Type FieldDescriptorAnnotationValue(Type thatClass)
        {
            // TODO: complete implementation (skipping implementation for now) 
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public List<FieldDescriptor> AttributeFieldDescriptors
        {
            get
            {
                return attributeFieldDescriptors;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<FieldDescriptor> ElementFieldOptimizations
        {
            get
            {
                return elementFieldDescriptors;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type DescribedClass
        {
            get
            {
                return describedClass;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TagName
        {
            get
            {
                return tagName;
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public string DescribedClassSimpleName
        {
            get
            {
                return describedClassSimpleName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ElementState Instance
        {
            get
            {
                return XMLTools.GetInstance(describedClass);
            }
        }

        /// <summary>
        /// 
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
