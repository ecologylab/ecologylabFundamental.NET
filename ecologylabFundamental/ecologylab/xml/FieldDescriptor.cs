﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.generic;
using ecologylabFundamental.ecologylab.xml.types;
using System.Reflection;
using ecologylabFundamental.ecologylab.xml.types.scalar;
using ecologylabFundamental.ecologylab.atttributes;
using System.Collections;

namespace ecologylabFundamental.ecologylab.xml
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldDescriptor : FieldTypes
    {
        private FieldInfo field;
        private String tagName;
        private List<String> otherTags;

        private ClassDescriptor declaringClassDescriptor;
        private int type;
        private ScalarType scalarType;

        private String[] format;
        private Boolean needsEscaping;
        private FieldDescriptor wrappedFD;

        private DictionaryList<String, ClassDescriptor> tagClassDescriptors;
        private Dictionary<String, Type> tagClasses;

        private String collectionOrMapTagName;
        private Boolean wrapped;
        private MethodInfo setValueMethod;

        private ClassDescriptor elementClassDescriptor;
        private Type elementClass;

        private FieldInfo xmlTextScalarField;
        
        private Boolean isCDATA;       
        private ClassDescriptor classDescriptor;
        private FieldInfo thatField;
        private FieldDescriptor fieldDescriptor;

        const String START_CDATA = "<![CDATA[";
        const String END_CDATA = "]]>";

        /// <summary>
        /// 
        /// </summary>
        public FieldDescriptor()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseClassDescriptor"></param>
        public FieldDescriptor(ClassDescriptor baseClassDescriptor)
        {
            this.declaringClassDescriptor = baseClassDescriptor;
            this.field = null;
            this.tagName = baseClassDescriptor.TagName;
            this.type = PSEUDO_FIELD_DESCRIPTOR;
            this.scalarType = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="declaringClassDescriptor"></param>
        /// <param name="field"></param>
        /// <param name="annotationType"></param>
        public FieldDescriptor(ClassDescriptor declaringClassDescriptor, FieldInfo field, int annotationType)
        {
            this.declaringClassDescriptor = declaringClassDescriptor;
            this.field = field;

            DeriveTagClassDescriptors(field);
            this.tagName = XMLTools.GetXmlTagName(field);

            type = UNSET_TYPE;	
            type = DeriveTypeFromField(field, annotationType);

            switch (type)
            {
                case ATTRIBUTE:
                case LEAF: 
                case TEXT_ELEMENT:
                    scalarType = DeriveScalar(field); break;                
            }

            //setValueMethod = ReflectionTools.getMethod(field.getType(), "setValue", SET_METHOD_ARG);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseClassDescriptor"></param>
        /// <param name="wrappedFD"></param>
        /// <param name="wrapperTag"></param>
        public FieldDescriptor(ClassDescriptor baseClassDescriptor, FieldDescriptor wrappedFD, string wrapperTag)
        {
            this.declaringClassDescriptor = baseClassDescriptor;
            this.wrappedFD = wrappedFD;
            this.type = WRAPPER;
            this.tagName = wrapperTag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public FieldDescriptor(String tag)
        {
            this.tagName = tag;
            this.type = IGNORED_ELEMENT;
            this.field = null;
            this.declaringClassDescriptor = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private ScalarType DeriveScalar(FieldInfo field)
        {
            ScalarType result = TypeRegistry.GetType(field);
            if (result != null)
            {
                if (type == LEAF || type == TEXT_ELEMENT)
                {
                    isCDATA = XMLTools.LeafIsCDATA(field);
                    needsEscaping = result.NeedsEscaping;
                }
                format = XMLTools.GetFormatAnnotation(field);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="annotationType"></param>
        /// <returns></returns>
        private int DeriveTypeFromField(FieldInfo field, int annotationType)
        {
            int result = annotationType;
            Type fieldClass = field.FieldType;

            switch (annotationType)
            {
                case ATTRIBUTE:
                    scalarType = DeriveScalar(field);
                    if (scalarType == null)
                        result = IGNORED_ATTRIBUTE;
                    break;
                case LEAF:
                    scalarType = DeriveScalar(field);
                    if (scalarType == null)
                        result = IGNORED_ELEMENT;
                    break;
                case TEXT_ELEMENT:
                    scalarType = DeriveScalar(field);
                    if (scalarType == null)
                        result = IGNORED_ELEMENT;
                    break;
                case NESTED_ELEMENT:
                    if (!CheckAssignableFrom(typeof(ElementState), field, fieldClass, "xml_nested"))
                        result = IGNORED_ELEMENT;
                    else if (!IsPolymorphic)
                    {
                        elementClassDescriptor = ClassDescriptor.GetClassDescriptor(fieldClass);
                        elementClass = elementClassDescriptor.DescribedClass;
                        tagName = XMLTools.GetXmlTagName(field);
                    }
                    break;
                case COLLECTION_ELEMENT:
                    if (XMLTools.IsAnnotationPresent(field, typeof(xml_collection)))
                    {
                        xml_collection collectionAttribute = null;
                        collectionAttribute = (xml_collection)XMLTools.GetAnnotation(field, typeof(xml_collection));
                        String collectionTag = collectionAttribute.TagName;

                        if (!CheckAssignableFrom(typeof(IList), field, fieldClass, "xml_collection"))
                            return IGNORED_ATTRIBUTE;

                        if (!IsPolymorphic)
                        {
                            Type collectionElementClass = GetTypeArgClass(field, 0); // 0th type arg for Collection<FooState>

                            if (collectionTag == null || collectionTag.Length < 0)
                            {
                                Console.WriteLine("In " + declaringClassDescriptor.DescribedClass
                                        + "\n\tCan't translate  xml_collection " + field.Name
                                        + " because its tag argument is missing.");
                                return IGNORED_ELEMENT;
                            }
                            if (collectionElementClass == null)
                            {
                                Console.WriteLine("In " + declaringClassDescriptor.DescribedClass
                                        + "\n\tCan't translate  xml_collection " + field.Name
                                        + " because the parameterized type argument for the Collection is missing.");
                                return IGNORED_ELEMENT;
                            }
                            if (typeof(ElementState).IsAssignableFrom(collectionElementClass))
                            {
                                elementClassDescriptor = ClassDescriptor.GetClassDescriptor(collectionElementClass);
                                elementClass = elementClassDescriptor.DescribedClass;
                            }
                            else
                            {
                                result = COLLECTION_SCALAR;
                                scalarType = DeriveCollectionScalar(collectionElementClass, field);
                            }
                        }
                        else
                        {
                            if (collectionTag != null && collectionTag.Length > 0)
                            {
                                Console.WriteLine("In " + declaringClassDescriptor.DescribedClass
                                        + "\n\tIgnoring argument to  xml_collection " + field.Name
                                        + " because it is declared polymorphic with xml_classes.");
                            }
                        }
                        collectionOrMapTagName = collectionTag;
                    }
                    else
                        return IGNORED_ATTRIBUTE;
                    break;
                case MAP_ELEMENT:
                    if (XMLTools.IsAnnotationPresent(field, typeof(xml_map)))
                    {
                        xml_map mapAttribute = null;
                        mapAttribute = (xml_map)XMLTools.GetAnnotation(field, typeof(xml_map));
                        String mapTag = mapAttribute.TagName;

                        if (!CheckAssignableFrom(typeof(IList), field, fieldClass, "xml_collection"))
                            return IGNORED_ATTRIBUTE;

                        if (!IsPolymorphic)
                        {
                            Type mapElementClass = GetTypeArgClass(field, 0); // 0th type arg for Collection<FooState>

                            if (mapTag == null || mapTag.Length < 0)
                            {
                                Console.WriteLine("In " + declaringClassDescriptor.DescribedClass
                                        + "\n\tCan't translate  xml_map " + field.Name
                                        + " because its tag argument is missing.");
                                return IGNORED_ELEMENT;
                            }
                            if (mapElementClass == null)
                            {
                                Console.WriteLine("In " + declaringClassDescriptor.DescribedClass
                                        + "\n\tCan't translate  xml_map " + field.Name
                                        + " because the parameterized type argument for the Collection is missing.");
                                return IGNORED_ELEMENT;
                            }
                            if (typeof(ElementState).IsAssignableFrom(mapElementClass))
                            {
                                elementClassDescriptor = ClassDescriptor.GetClassDescriptor(mapElementClass);
                                elementClass = elementClassDescriptor.DescribedClass;
                            }
                            else
                            {
                                result = COLLECTION_SCALAR;
                                scalarType = DeriveCollectionScalar(mapElementClass, field);
                            }
                        }
                        else
                        {
                            if (mapTag != null && mapTag.Length > 0)
                            {
                                Console.WriteLine("In " + declaringClassDescriptor.DescribedClass
                                        + "\n\tIgnoring argument to  xml_map " + field.Name
                                        + " because it is declared polymorphic with xml_classes.");
                            }
                        }
                        collectionOrMapTagName = mapTag;
                    }
                    else
                        return IGNORED_ATTRIBUTE;
                    break;
                default:
                    break;
            }

            if (annotationType == COLLECTION_ELEMENT || annotationType == MAP_ELEMENT)
            {
                if (!XMLTools.IsAnnotationPresent(field, typeof(xml_nowrap)))
                    wrapped = true;
            }
            if (result == UNSET_TYPE)
            {
                Console.WriteLine("Programmer error -- can't derive type.");
                result = IGNORED_ELEMENT;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetClass"></param>
        /// <param name="field"></param>
        /// <param name="fieldClass"></param>
        /// <param name="annotationDescription"></param>
        /// <returns></returns>
        private bool CheckAssignableFrom(Type targetClass, FieldInfo field, Type fieldClass, string annotationDescription)
        {
            Boolean result = targetClass.IsAssignableFrom(fieldClass);
            if (!result)
            {
                System.Console.WriteLine("In " + declaringClassDescriptor.DescribedClass
                        + "\n\tCan't translate  " + annotationDescription + "() " + field.Name
                        + " because the annotated field is not an instance of " + targetClass.Name + ".");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionScalarClass"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private ScalarType DeriveCollectionScalar(Type collectionScalarClass, FieldInfo field)
        {
            ScalarType result = TypeRegistry.GetType(collectionScalarClass);
            if (result == null)
            {
                needsEscaping = result.NeedsEscaping;
                format = XMLTools.GetFormatAnnotation(field);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private Type GetTypeArgClass(FieldInfo field, int p)
        {
            Type result = null;
            Type t = field.FieldType;

            Type[] typeArgs = t.GetGenericArguments();

            if (typeArgs != null)
            {
                int max = typeArgs.Length - 1;
                if (p > max)
                {
                    p = max;
                }

                Type typeArg0 = typeArgs[p];
                if (typeArg0 is Type)
                {
                    result = typeArg0;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        private void DeriveTagClassDescriptors(FieldInfo field)
        {   
		    if (XMLTools.IsAnnotationPresent(field, typeof(xml_scope)))
		    {
                xml_scope scopeAnnotationObj = (xml_scope)XMLTools.GetAnnotation(field, typeof(xml_scope));
                String scopeAnnotation = scopeAnnotationObj.TranslationScope;
			    TranslationScope scope = TranslationScope.Get(scopeAnnotation);
			    if (scope != null)
			    {
				    List<ClassDescriptor> scopeClassDescriptors = scope.ClassDescriptors;
				    InitTagClassDescriptorsArrayList(scopeClassDescriptors.Count);
                    foreach (ClassDescriptor classDescriptor in scopeClassDescriptors)
                    {
                        tagClassDescriptors.Add(classDescriptor.TagName, classDescriptor);
                        tagClasses.Add(classDescriptor.TagName, classDescriptor.DescribedClass);
                    }
					
			    }
		    }
            else if (XMLTools.IsAnnotationPresent(field, typeof(xml_classes)))
            {
                xml_classes classesAnnotationObj = (xml_classes)XMLTools.GetAnnotation(field, typeof(xml_classes));
                Type[] classesAnnotation = classesAnnotationObj.Classes;
                InitTagClassDescriptorsArrayList(classesAnnotation.Length);
                foreach (Type thatClass in classesAnnotation)
                    if (typeof(ElementState).IsAssignableFrom(thatClass))
                    {
                        ClassDescriptor classDescriptor = ClassDescriptor.GetClassDescriptor(thatClass);
                        tagClassDescriptors.Add(classDescriptor.TagName, classDescriptor);
                        tagClasses.Add(classDescriptor.TagName, classDescriptor.DescribedClass);
                    }
            }
            else if (XMLTools.IsAnnotationPresent(field, typeof(xml_class)))
            {
                xml_class classAnnotationObj = (xml_class)XMLTools.GetAnnotation(field, typeof(xml_class));
                Type classAnnotation = classAnnotationObj.Class;
                tagClassDescriptors.Add(classDescriptor.TagName, classDescriptor);
                tagClasses.Add(classDescriptor.TagName, classDescriptor.DescribedClass);
            }
        }

        private void InitTagClassDescriptorsArrayList(int initialSize)
        {
            if (tagClassDescriptors == null)
            {
                tagClassDescriptors = new DictionaryList<string, ClassDescriptor>(initialSize);
            }
            if (tagClasses == null)
            {
                tagClasses = new Dictionary<String, Type>(initialSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffy"></param>
        public void WriteElementStart(StringBuilder buffy)
        {
            buffy.Append('<').Append(ElementStart);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="context"></param>
        public void AppendValueAsAttribute(StringBuilder buffy, ElementState context)
        {
            if (context != null)
            {
                ScalarType scalarType = this.scalarType;
                FieldInfo field = this.field;

                if (scalarType == null)
                {
                    Console.WriteLine("scalarType = null!");
                }
                else if (!scalarType.IsDefaultValue(field, context))
                {
                    buffy.Append(' ');
                    buffy.Append(this.tagName);
                    buffy.Append('=');
                    buffy.Append('"');

                    scalarType.AppendValue(buffy, this, context);
                    buffy.Append('"');
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean HasXmlText
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsXmlNsDecl
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="elementState"></param>
        public void AppendXmlText(StringBuilder output, ElementState elementState)
        {
            if (elementState != null)
            {
                ScalarType scalarType = this.scalarType;
                if (!scalarType.IsDefaultValue(xmlTextScalarField, elementState))
                {
                    if (isCDATA)
                        output.Append(START_CDATA);
                    scalarType.AppendValue(output, this, elementState);
                    if (isCDATA)
                        output.Append(END_CDATA);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="elementState"></param>
        public void AppendLeaf(StringBuilder output, ElementState elementState)
        {
            if (elementState != null)
            {
                ScalarType scalarType = this.scalarType;
                FieldInfo field = this.field;

                if (!scalarType.IsDefaultValue(field, elementState))
                {
                    WriteOpenTag(output);

                    if (isCDATA)
                        output.Append(START_CDATA);
                    scalarType.AppendValue(output, this, elementState); // escape if not CDATA! :-)
                    if (isCDATA)
                        output.Append(END_CDATA);

                    WriteCloseTag(output);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        private void WriteOpenTag(StringBuilder output)
        {
            output.Append('<').Append(ElementStart).Append('>');
        }              

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        public void WriteCloseTag(StringBuilder output)
        {
            output.Append('<').Append('/').Append(ElementStart).Append('>');
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsWrapped
        {
            get
            {
                return wrapped;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="close"></param>
        public void WriteWrap(StringBuilder output, bool close)
        {
            output.Append('<');
            if (close)
                output.Append('/');
            output.Append(tagName).Append('>');
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPolymorphic
        {
            get
            {
                return tagClassDescriptors != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="instance"></param>
        public void AppendCollectionLeaf(StringBuilder buffy, object instance)
        {
            if (instance != null)
            {
                ScalarType scalarType = this.scalarType;

                WriteOpenTag(buffy);
                if (isCDATA)
                    buffy.Append(START_CDATA);
                scalarType.AppendValue(instance, buffy, !isCDATA); 
                if (isCDATA)
                    buffy.Append(END_CDATA);

                WriteCloseTag(buffy);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetTypeId()
        {
            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        public String ElementStart
        {
            get
            {
                return IsCollection ? CollectionOrMapTagName : TagName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCollection
        {
            get
            {
                switch (type)
                {
                    case MAP_ELEMENT:
                    case MAP_SCALAR:
                    case COLLECTION_ELEMENT:
                    case COLLECTION_SCALAR:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CollectionOrMapTagName
        { 
            get 
            { 
                return collectionOrMapTagName; 
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String TagName
        {
            get
            {
                return tagName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldInfo Field
        {
             get
            {
                return field;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMarshallOnly
        {
            get
            {
                return scalarType != null && scalarType.IsMarshallOnly;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ClassDescriptor> TagClassDescriptors
        {
            get
            {
                return tagClassDescriptors.Values;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FieldName
        {
            get
            {
                return field.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCDATA
        {
            get
            {
                return isCDATA;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldDescriptor WrappedFieldDescriptor
        {
            get { return wrappedFD; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static FieldDescriptor IGNORED_ELEMENT_FIELD_DESCRIPTOR { get { return new FieldDescriptor("IGNORED"); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nestedObject"></param>
        public void SetFieldToNestedObject(ElementState context, ElementState nestedObject)
        {
            this.field.SetValue(context, nestedObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public ElementState ConstructChildElementState(ElementState parent, String tagName)
        {
            ClassDescriptor childClassDescriptor = !IsPolymorphic ? elementClassDescriptor : tagClassDescriptors[tagName];
            ElementState result = null;

            if (childClassDescriptor != null)
            {
                result = childClassDescriptor.Instance;
                if (result != null)
                {
                    parent.SetupChildElementState(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentElementState"></param>
        /// <returns></returns>
        public Object AutomaticLazyGetCollectionOrDict(ElementState currentElementState)
        {
            Object collection = null;

            collection = field.GetValue(currentElementState);

            if (collection == null)
            {
                Type collectionType = field.FieldType;
                collection = Activator.CreateInstance(collectionType);
                field.SetValue(currentElementState, collection);
            }

            return collection; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="scalarUnMarshallingContext"></param>
        public void SetFieldToScalar(ElementState context, String value, ElementStateSAXHandler scalarUnMarshallingContext)
        {
            if ((value == null))
            {
                return;
            }
            if (setValueMethod != null)
            {
                Object[] args = new Object[1];
                args[0] = value;

                setValueMethod.Invoke(context, args);
            }
            else if (scalarType != null && !scalarType.IsMarshallOnly)
            {
                scalarType.SetField(context, field, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeElementState"></param>
        /// <param name="leafNodeValue"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        public void AddLeafNodeToCollection(ElementState activeElementState, String leafNodeValue, ElementStateSAXHandler scalarUnmarshallingContext)
        {
            if (leafNodeValue != null)
            {
                //silently ignore the leaf node values. 
            }

            if (scalarType != null)
            {
                Object typeConvertedValue = scalarType.GetInstance(leafNodeValue, format);
                if (typeConvertedValue != null)
                {
                    IList collection = (IList)AutomaticLazyGetCollectionOrDict(activeElementState);
                    collection.Add(typeConvertedValue);
                }
            }
            else
            { 
                //TODO: report error
            }
        }
    }
}
