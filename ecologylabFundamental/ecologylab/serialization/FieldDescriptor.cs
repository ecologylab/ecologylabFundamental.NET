using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.generic;
using ecologylabFundamental.ecologylab.serialization.types;
using System.Reflection;
using ecologylabFundamental.ecologylab.serialization.types.scalar;
using ecologylabFundamental.ecologylab.atttributes;
using System.Collections;

namespace ecologylabFundamental.ecologylab.serialization
{
    /// <summary>
    ///     <c>FieldDescriptors</c> are abstract data strucutres which defines a field in a 
    ///     <see cref="ClassDescriptor"/>. Holds the binding information for marshalling 
    ///     and unmarshalling of fields to their XML representation.
    /// </summary>
    public class FieldDescriptor : FieldTypes
    {
        const String START_CDATA = "<![CDATA[";
        const String END_CDATA = "]]>";

        #region Private Fields

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
        private int fieldType;
        private Type fieldDescriptorClass;
        private Hint xmlHint;
        private object isEnum;

        #endregion

        #region Constructors

        /// <summary>
        ///     Default constructor
        /// </summary>
        public FieldDescriptor()
        {
        }


        /// <summary>
        ///     Constructing which a declaring class descriptor and initializes internal
        ///     data structure for the field descriptor. This constructor creates a 
        ///     Pseudo <c>FieldDescriptor</c>. This is used for polymorphic types.
        /// </summary>
        /// <param name="declaringClassDescriptor">
        ///     The <c>ClassDescriptor</c> for the class which defines the field
        ///     described by this <c>FieldDescriptor</c>.
        /// </param>
        public FieldDescriptor(ClassDescriptor declaringClassDescriptor)
        {
            this.declaringClassDescriptor = declaringClassDescriptor;
            this.field = null;
            this.tagName = declaringClassDescriptor.TagName;
            this.type = PSEUDO_FIELD_DESCRIPTOR;
            this.scalarType = null;
        }

        /// <summary>   
        ///     Creates a <c>FieldDescriptor</c> object from the field and annotation type
        /// </summary>
        /// <param name="declaringClassDescriptor">
        ///     The <c>ClassDescriptor</c> for the class which defines the field
        ///     described by this <c>FieldDescriptor</c>.
        /// </param>
        /// <param name="field">
        ///     <c>FieldInfo</c> contains the reflected information about a field.
        /// </param>
        /// <param name="annotationType">
        ///     <c>Int16</c> type id of the annotation.
        /// </param>
        public FieldDescriptor(ClassDescriptor declaringClassDescriptor, FieldInfo field, Int16 annotationType)
        {
            this.declaringClassDescriptor = declaringClassDescriptor;
            this.field = field;

            DeriveTagClassDescriptors(field);
            this.tagName = XMLTools.GetXmlTagName(field);

            type = UNSET_TYPE;

            type = UNSET_TYPE; // for debugging!

            if (annotationType == SCALAR)
                type = DeriveScalarSerialization(field);
            else
                type = DeriveNestedSerialization(field, annotationType);

            //TODO: if we case use the set method in there? 
            //setValueMethod = ReflectionTools.getMethod(field.getType(), "setValue", SET_METHOD_ARG);
        }       

        /// <summary>
        ///     Creates a <c>FieldDescriptor</c> for wrapping tag names in XML files.
        /// </summary>
        /// <param name="declaringClassDescriptor">
        ///     The <c>ClassDescriptor</c> for the class which defines the field
        ///     described by this <c>FieldDescriptor</c>.
        /// </param>
        /// <param name="wrappedFD">
        ///     <c>FieldDescriptor</c> for the wrapping tag
        /// </param>
        /// <param name="wrapperTag">
        ///     <c>String</c> wrapper tag name
        /// </param>
        public FieldDescriptor(ClassDescriptor declaringClassDescriptor, FieldDescriptor wrappedFD, String wrapperTag)
        {
            this.declaringClassDescriptor = declaringClassDescriptor;
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

        #endregion

        #region Public Methods

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
        public void SetFieldToScalar(ElementState context, String value)
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

        #endregion 

        #region Private Methods
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private int DeriveScalarSerialization(FieldInfo field)
        {
            int result = DeriveScalarSerialization(field.FieldType, field);
            if (xmlHint == Hint.XML_TEXT || xmlHint == Hint.XML_LEAF_CDATA)
                this.declaringClassDescriptor.ScalarTextFD = this;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private int DeriveScalarSerialization(System.Type thatClass, FieldInfo field)
        {
            isEnum = XMLTools.IsEnum(field);
            xmlHint = XMLTools.SimplHint(field);
            scalarType = TypeRegistry.GetType(thatClass);

            if (scalarType == null)
            {
                String message = "Can't find ScalarType to serialize field: \t\t" + thatClass.Name + "\t" + field.Name + ";";
                Console.WriteLine(message);
                return (xmlHint == Hint.XML_ATTRIBUTE) ? IGNORED_ATTRIBUTE : IGNORED_ELEMENT;
            }

            if (xmlHint != Hint.XML_ATTRIBUTE)
            {
                needsEscaping = scalarType.NeedsEscaping;
                isCDATA = xmlHint == Hint.XML_LEAF_CDATA || xmlHint == Hint.XML_TEXT_CDATA;
            }

            //TODO : simple filter annotation;

            return SCALAR;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="annotationType"></param>
        /// <returns></returns>
        private int DeriveNestedSerialization(FieldInfo field, int annotationType)
        {
            int result = annotationType;
            Type fieldClass = field.FieldType;

            switch (annotationType)
            {               
                case COMPOSITE_ELEMENT:
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
                    if (XMLTools.IsAnnotationPresent(field, typeof(simpl_collection)))
                    {
                        simpl_collection collectionAttribute = null;
                        collectionAttribute = (simpl_collection)XMLTools.GetAnnotation(field, typeof(simpl_collection));
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
                    if (XMLTools.IsAnnotationPresent(field, typeof(simpl_map)))
                    {
                        simpl_map mapAttribute = null;
                        mapAttribute = (simpl_map)XMLTools.GetAnnotation(field, typeof(simpl_map));
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
                if (!XMLTools.IsAnnotationPresent(field, typeof(simpl_nowrap)))
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
            if (XMLTools.IsAnnotationPresent(field, typeof(simpl_scope)))
            {
                simpl_scope scopeAnnotationObj = (simpl_scope)XMLTools.GetAnnotation(field, typeof(simpl_scope));
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
            else if (XMLTools.IsAnnotationPresent(field, typeof(simpl_classes)))
            {
                simpl_classes classesAnnotationObj = (simpl_classes)XMLTools.GetAnnotation(field, typeof(simpl_classes));
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialSize"></param>
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

        #endregion

        #region Properties

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
        public Hint XmlHint
        {
            get
            {
                return xmlHint;
            }
            set
            {
                xmlHint = value;
            }
        }

        #endregion
              
    }
}
