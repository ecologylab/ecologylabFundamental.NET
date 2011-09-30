using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.Context;
using Simpl.Serialization.Types;
using ecologylab.serialization.types;
using System.Reflection;
using ecologylab.serialization.types.scalar;
using System.Collections;
using System.Text.RegularExpressions;

namespace Simpl.Serialization
{
    /// <summary>
    ///     <c>FieldDescriptors</c> are abstract data strucutres which defines a field in a 
    ///     <see cref="ClassDescriptor"/>. Holds the binding information for marshalling 
    ///     and unmarshalling of fields to their XML representation.
    /// </summary>
    public class FieldDescriptor : FieldTypes
    {
        private readonly FieldInfo _field;
        private String _tagName;
        private List<String> _otherTags;

        private readonly ClassDescriptor _declaringClassDescriptor;
        private readonly int _type;
        private ScalarType _scalarType;

        private String[] _format;
        private Boolean _needsEscaping;
        private readonly FieldDescriptor _wrappedFD;

        private DictionaryList<String, ClassDescriptor> _tagClassDescriptors;
        private Dictionary<String, Type> _tagClasses;

        private String _collectionOrMapTagName;
        private Boolean _wrapped;
        private readonly MethodInfo _setValueMethod;

        private ClassDescriptor _elementClassDescriptor;
        private Type _elementClass;

        private readonly FieldInfo _xmlTextScalarField;

        private Boolean _isCdata;
        private FieldInfo _thatField;
        private int _fieldType;
        private Type _fieldDescriptorClass;
        private Hint _xmlHint;
        private object _isEnum;

	    Regex   regex;

	    String  filterReplace;

        private String unresolvedScopeAnnotation = null;
        private string compositeTagName;
       

     
        /// <summary>
        ///     Default constructor
        /// </summary>
        public FieldDescriptor(MethodInfo setValueMethod, FieldInfo xmlTextScalarField, FieldInfo xmlTextScalarField)
        {
            this._setValueMethod = setValueMethod;
            this._xmlTextScalarField = xmlTextScalarField;
            this._xmlTextScalarField = xmlTextScalarField;
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
        public FieldDescriptor(ClassDescriptor declaringClassDescriptor, MethodInfo setValueMethod, FieldInfo xmlTextScalarField, FieldInfo xmlTextScalarField)
        {
            this._declaringClassDescriptor = declaringClassDescriptor;
            this._setValueMethod = setValueMethod;
            this._xmlTextScalarField = xmlTextScalarField;
            this._xmlTextScalarField = xmlTextScalarField;
            this._field = null;
            this._tagName = declaringClassDescriptor.TagName;
            this._type = Pseudo;
            this._scalarType = null;
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
        public FieldDescriptor(ClassDescriptor declaringClassDescriptor, FieldInfo field, Int16 annotationType, MethodInfo setValueMethod, FieldInfo xmlTextScalarField, FieldInfo xmlTextScalarField)
        {
            this._declaringClassDescriptor = declaringClassDescriptor;
            this._field = field;
            this._setValueMethod = setValueMethod;
            this._xmlTextScalarField = xmlTextScalarField;
            this._xmlTextScalarField = xmlTextScalarField;

            DeriveTagClassDescriptors(field);
            this._tagName = XmlTools.GetXmlTagName(field);

            _type = UnsetType;

            _type = UnsetType; // for debugging!

            if (annotationType == Scalar)
                _type = DeriveScalarSerialization(field);
            else
                _type = DeriveNestedSerialization(field, annotationType);

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
        public FieldDescriptor(ClassDescriptor declaringClassDescriptor, FieldDescriptor wrappedFD, String wrapperTag, MethodInfo setValueMethod, FieldInfo xmlTextScalarField, FieldInfo xmlTextScalarField)
        {
            this._declaringClassDescriptor = declaringClassDescriptor;
            this._wrappedFD = wrappedFD;
            this._type = Wrapper;
            this._tagName = wrapperTag;
            this._setValueMethod = setValueMethod;
            this._xmlTextScalarField = xmlTextScalarField;
            this._xmlTextScalarField = xmlTextScalarField;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public FieldDescriptor(String tag, MethodInfo setValueMethod, FieldInfo xmlTextScalarField, FieldInfo xmlTextScalarField)
        {
            this._tagName = tag;
            this._setValueMethod = setValueMethod;
            this._xmlTextScalarField = xmlTextScalarField;
            this._xmlTextScalarField = xmlTextScalarField;
            this._type = IgnoredElement;
            this._field = null;
            this._declaringClassDescriptor = null;
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
        /// Note: XML only method.
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="context"></param>
        public void AppendValueAsAttribute(StringBuilder buffy, ElementState context, TranslationContext serializationContext)
        {
            if (context != null)
            {
                ScalarType scalarType = this._scalarType;
                FieldInfo field = this._field;

                if (scalarType == null)
                {
                    Console.WriteLine("scalarType = null!");
                }
                else if (!scalarType.IsDefaultValue(field, context))
                {
                    buffy.Append(' ');
                    buffy.Append(this._tagName);
                    buffy.Append('=');
                    buffy.Append('"');

                    scalarType.AppendValue(buffy, this, context, Format.Xml);
                    buffy.Append('"');
                }
            }
        }

        /// <summary>
        /// Note: XML only method.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="elementState"></param>
        public void AppendXmlText(StringBuilder output, ElementState elementState)
        {
            if (elementState != null)
            {
                ScalarType scalarType = this._scalarType;
                if (!scalarType.IsDefaultValue(_xmlTextScalarField, elementState))
                {
                    if (_isCdata)
                        output.Append(START_CDATA);
                    scalarType.AppendValue(output, this, elementState, Format.XML);
                    if (_isCdata)
                        output.Append(END_CDATA);
                }
            }
        }

        /// <summary>
        /// Note: XML only method.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="elementState"></param>
        public void AppendLeaf(StringBuilder output, ElementState elementState, TranslationContext serializationContext)
        {
            if (elementState != null)
            {
                ScalarType scalarType = this._scalarType;
                FieldInfo field = this._field;

                if (!scalarType.IsDefaultValue(field, elementState))
                {
                    WriteOpenTag(output);

                    if (_isCdata)
                        output.Append(START_CDATA);
                    scalarType.AppendValue(output, this, elementState, Format.XML); // escape if not CDATA! :-)
                    if (_isCdata)
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
            output.Append(_tagName).Append('>');
        }

        /// <summary>
        /// Note: XML only method.
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="instance"></param>
        public void AppendCollectionLeaf(StringBuilder buffy, object instance)
        {
            if (instance != null)
            {
                ScalarType scalarType = this._scalarType;

                WriteOpenTag(buffy);
                if (_isCdata)
                    buffy.Append(START_CDATA);
                scalarType.AppendValue(instance, buffy, !_isCdata, Format.XML);
                if (_isCdata)
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
            this._field.SetValue(context, nestedObject);
        }

        /// <summary>
        /// Construct child elementstate object based on the given grpahcontext
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public ElementState ConstructChildElementState(ElementState parent, String tagName, ecologylab.serialization.sax.Attributes attributes, TranslationContext graphContext)
        {
            if (_tagClassDescriptors != null && !_tagClassDescriptors.ContainsKey(tagName))
                Console.WriteLine("Error: " + tagName);
            ClassDescriptor childClassDescriptor = !IsPolymorphic ? _elementClassDescriptor : _tagClassDescriptors[tagName];
            ElementState result = null;

            if (childClassDescriptor != null)
            {
                result = GetInstance(attributes, childClassDescriptor, graphContext);
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
        /// <param name="parent"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public ElementState ConstructChildElementState(ElementState parent, String tagName)
        {
            if (_tagClassDescriptors != null && !_tagClassDescriptors.ContainsKey(tagName))
                Console.WriteLine("Error: " + tagName);
            ClassDescriptor childClassDescriptor = !IsPolymorphic ? _elementClassDescriptor : _tagClassDescriptors[tagName];
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

            collection = _field.GetValue(currentElementState);

            if (collection == null)
            {
                Type collectionType = _field.FieldType;
                collection = Activator.CreateInstance(collectionType);
                _field.SetValue(currentElementState, collection);
            }

            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>        
        /// <param name="scalarUnmarshallingContext"></param>
        public void SetFieldToScalar(ElementState context, String value, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            if (value == null)
                return;


            if (_setValueMethod != null)
            {
                Object[] args = new Object[1];
                args[0] = value;

                _setValueMethod.Invoke(context, args);
            }
            else if (_scalarType != null && !_scalarType.IsMarshallOnly)
            {
                var UGLY_UNESCAPING = new StringBuilder(value).Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'");
                _scalarType.SetField(context, _field, UGLY_UNESCAPING.ToString(), null, scalarUnmarshallingContext);
                //scalarType.SetField(context, field, value, null, scalarUnmarshallingContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeElementState"></param>
        /// <param name="leafNodeValue"></param>
        /// 
        public void AddLeafNodeToCollection(ElementState activeElementState, String leafNodeValue, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            if (leafNodeValue != null)
            {
                //silently ignore the leaf node values. 
            }

            if (_scalarType != null)
            {
                Object typeConvertedValue = _scalarType.GetInstance(leafNodeValue, _format, scalarUnmarshallingContext);
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
            if (_xmlHint == Hint.XmlText || _xmlHint == Hint.XmlLeafCdata)
                this._declaringClassDescriptor.ScalarTextFD = this;
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
            _isEnum = XmlTools.IsEnum(field);
            _xmlHint = XmlTools.SimplHint(field);
            _scalarType = TypeRegistry.GetType(thatClass);

            if (_scalarType == null)
            {
                String message = "Can't find ScalarType to serialize field: \t\t" + thatClass.Name + "\t" + field.Name + ";";
                Console.WriteLine(message);
                return (_xmlHint == Hint.XmlAttribute) ? IgnoredAttribute : IgnoredElement;
            }

            if (_xmlHint != Hint.XmlAttribute)
            {
                _needsEscaping = _scalarType.NeedsEscaping;
                _isCdata = _xmlHint == Hint.XmlLeafCdata || _xmlHint == Hint.XmlTextCdata;
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
                case CompositeElement:
                    if (!CheckAssignableFrom(typeof(ElementState), field, fieldClass, "xml_nested"))
                        result = IgnoredElement;
                    else if (!IsPolymorphic)
                    {
                        _elementClassDescriptor = ClassDescriptor.GetClassDescriptor(fieldClass);
                        _elementClass = _elementClassDescriptor.DescribedClass;
                        _tagName = XmlTools.GetXmlTagName(field);
                    }
                    break;
                case CollectionElement:
                    if (XmlTools.IsAnnotationPresent(field, typeof(SimplCollection)))
                    {
                        SimplCollection collectionAttribute = null;
                        collectionAttribute = (SimplCollection)XmlTools.GetAnnotation(field, typeof(SimplCollection));
                        String collectionTag = collectionAttribute.TagName;

                        if (!CheckAssignableFrom(typeof(IList), field, fieldClass, "xml_collection"))
                            return IgnoredAttribute;

                        if (!IsPolymorphic)
                        {
                            Type collectionElementClass = GetTypeArgClass(field, 0); // 0th type arg for Collection<FooState>

                            if (collectionTag == null || collectionTag.Length < 0)
                            {
                                Console.WriteLine("In " + _declaringClassDescriptor.DescribedClass
                                        + "\n\tCan't translate  xml_collection " + field.Name
                                        + " because its tag argument is missing.");
                                return IgnoredElement;
                            }
                            if (collectionElementClass == null)
                            {
                                Console.WriteLine("In " + _declaringClassDescriptor.DescribedClass
                                        + "\n\tCan't translate  xml_collection " + field.Name
                                        + " because the parameterized type argument for the Collection is missing.");
                                return IgnoredElement;
                            }
                            if (typeof(ElementState).IsAssignableFrom(collectionElementClass))
                            {
                                _elementClassDescriptor = ClassDescriptor.GetClassDescriptor(collectionElementClass);
                                _elementClass = _elementClassDescriptor.DescribedClass;
                            }
                            else
                            {
                                result = CollectionScalar;
                                _scalarType = DeriveCollectionScalar(collectionElementClass, field);
                            }
                        }
                        else
                        {
                            if (collectionTag != null && collectionTag.Length > 0)
                            {
                                Console.WriteLine("In " + _declaringClassDescriptor.DescribedClass
                                        + "\n\tIgnoring argument to  xml_collection " + field.Name
                                        + " because it is declared polymorphic with xml_classes.");
                            }
                        }
                        _collectionOrMapTagName = collectionTag;
                    }
                    else
                        return IgnoredAttribute;
                    break;
                case MapElement:
                    if (XmlTools.IsAnnotationPresent(field, typeof(SimplMap)))
                    {
                        SimplMap mapAttribute = null;
                        mapAttribute = (SimplMap)XmlTools.GetAnnotation(field, typeof(SimplMap));
                        String mapTag = mapAttribute.TagName;

                        if (!CheckAssignableFrom(typeof(IDictionary), field, fieldClass, "xml_collection"))
                            return IgnoredAttribute;

                        if (!IsPolymorphic)
                        {
                            Type mapElementClass = GetTypeArgClass(field, 1); // 0th type arg for Collection<FooState>

                            if (mapTag == null || mapTag.Length < 0)
                            {
                                Console.WriteLine("In " + _declaringClassDescriptor.DescribedClass
                                        + "\n\tCan't translate  xml_map " + field.Name
                                        + " because its tag argument is missing.");
                                return IgnoredElement;
                            }
                            if (mapElementClass == null)
                            {
                                Console.WriteLine("In " + _declaringClassDescriptor.DescribedClass
                                        + "\n\tCan't translate  xml_map " + field.Name
                                        + " because the parameterized type argument for the Collection is missing.");
                                return IgnoredElement;
                            }
                            if (typeof(ElementState).IsAssignableFrom(mapElementClass))
                            {
                                _elementClassDescriptor = ClassDescriptor.GetClassDescriptor(mapElementClass);
                                _elementClass = _elementClassDescriptor.DescribedClass;
                            }
                            else
                            {
                                result = CollectionScalar;
                                _scalarType = DeriveCollectionScalar(mapElementClass, field);
                            }
                        }
                        else
                        {
                            if (mapTag != null && mapTag.Length > 0)
                            {
                                Console.WriteLine("In " + _declaringClassDescriptor.DescribedClass
                                        + "\n\tIgnoring argument to  xml_map " + field.Name
                                        + " because it is declared polymorphic with xml_classes.");
                            }
                        }
                        _collectionOrMapTagName = mapTag;
                    }
                    else
                        return IgnoredAttribute;
                    break;
                default:
                    break;
            }

            if (annotationType == CollectionElement || annotationType == MapElement)
            {
                if (!XmlTools.IsAnnotationPresent(field, typeof(SimplNowrap)))
                    _wrapped = true;
            }
            if (result == UnsetType)
            {
                Console.WriteLine("Programmer error -- can't derive type.");
                result = IgnoredElement;
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
                System.Console.WriteLine("In " + _declaringClassDescriptor.DescribedClass
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
            if (result != null)
            {
                _needsEscaping = result.NeedsEscaping;
                _format = XmlTools.GetFormatAnnotation(field);
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
            if (XmlTools.IsAnnotationPresent(field, typeof(SimplScope)))
            {
                SimplScope scopeAnnotationObj = (SimplScope)XmlTools.GetAnnotation(field, typeof(SimplScope));
                String scopeAnnotation = scopeAnnotationObj.TranslationScope;

                if (scopeAnnotation != null && scopeAnnotation.Length > 0)
                {
                    if (!ResolveScopeAnnotation(scopeAnnotation))
                    {
                        unresolvedScopeAnnotation = scopeAnnotation;
                        _declaringClassDescriptor.RegisterUnresolvedScopeAnnotationFD(this);
                    }
                }

               
            }
            else if (XmlTools.IsAnnotationPresent(field, typeof(SimplClasses)))
            {
                SimplClasses classesAnnotationObj = (SimplClasses)XmlTools.GetAnnotation(field, typeof(SimplClasses));
                Type[] classesAnnotation = classesAnnotationObj.Classes;
                InitTagClassDescriptorsArrayList(classesAnnotation.Length);
                foreach (Type thatClass in classesAnnotation)
                    if (typeof(ElementState).IsAssignableFrom(thatClass))
                    {
                        ClassDescriptor classDescriptor = ClassDescriptor.GetClassDescriptor(thatClass);
                        ClassDescriptor previousMapping = null;
                        if (_tagClassDescriptors.TryGetValue(classDescriptor.TagName, out previousMapping))
                        {
                            _tagClassDescriptors.Remove(classDescriptor.TagName);
                        }
                        _tagClassDescriptors.Add(classDescriptor.TagName, classDescriptor);

                        Type previousType = null;

                        if (_tagClasses.TryGetValue(classDescriptor.TagName, out previousType))
                        {
                            _tagClasses.Remove(classDescriptor.TagName);
                        }
                        _tagClasses.Add(classDescriptor.TagName, classDescriptor.DescribedClass);
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialSize"></param>
        private void InitTagClassDescriptorsArrayList(int initialSize)
        {
            if (_tagClassDescriptors == null)
            {
                _tagClassDescriptors = new DictionaryList<string, ClassDescriptor>(initialSize);
            }
            if (_tagClasses == null)
            {
                _tagClasses = new Dictionary<String, Type>(initialSize);
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
                return _tagClassDescriptors != null || unresolvedScopeAnnotation != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsWrapped
        {
            get
            {
                return _wrapped;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetTypeId()
        {
            return _type;
        }

        /// <summary>
        /// 
        /// </summary>
        public String ElementStart
        {
            get
            {
                return IsCollection ? CollectionOrMapTagName : IsNested ? compositeTagName : _tagName;
            }
        }

        protected bool IsNested
        {
            get { return _type == CompositeElement; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCollection
        {
            get
            {
                switch (_type)
                {
                    case MapElement:
                    case CollectionElement:
                    case CollectionScalar:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool IsScalar
        {
            get { return _scalarType != null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CollectionOrMapTagName
        {
            get
            {
                return _collectionOrMapTagName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String TagName
        {
            get
            {
                return _tagName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldInfo Field
        {
            get
            {
                return _field;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMarshallOnly
        {
            get
            {
                return _scalarType != null && _scalarType.IsMarshallOnly;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DictionaryList<String, ClassDescriptor> TagClassDescriptors
        {
            get
            {
                return _tagClassDescriptors;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FieldName
        {
            get
            {
                return _field == null ? null : _field.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCDATA
        {
            get
            {
                return _isCdata;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldDescriptor WrappedFieldDescriptor
        {
            get { return _wrappedFD; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Hint XmlHint
        {
            get
            {
                return _xmlHint;
            }
            set
            {
                _xmlHint = value;
            }
        }

        public ClassDescriptor ClassDescriptor
        {
            get { return _elementClassDescriptor; }
        }

        #endregion


        public void WriteJSONElementStart(StringBuilder output, bool withTag)
        {
            if (withTag)
            {
                output.Append('"').Append(ElementStart).Append('"');
                output.Append(':');
            }
            output.Append('{');
        }

        public bool IsDefaultValue(ElementState context)
        {
            if (context != null)
                return _scalarType.IsDefaultValue(this._field, context);
            return false;
        }

        ///<summary>
        /// Note: JSON Only method
        ///</summary>
        ///<param name="output"></param>
        ///<param name="context"></param>
        ///<param name="isFirst"></param>
        public void AppendValueAsJSONAttribute(StringBuilder output, ElementState context, bool isFirst)
        {
            if (context != null)
            {
                ScalarType scalarType = this._scalarType;
                FieldInfo field = this._field;

                if (!scalarType.IsDefaultValue(field, context))
                {
                    if (!isFirst)
                        output.Append(", ");

                    output.Append('"');
                    output.Append(_tagName);
                    output.Append('"');
                    output.Append(':');
                    output.Append('"');

                    scalarType.AppendValue(output, this, context, Format.JSON);
                    output.Append('"');

                }
            }
        }

        public void WriteJSONWrap(StringBuilder output, bool close)
        {
            if (!close)
            {
                output.Append('"');
                output.Append(_tagName);
                output.Append('"').Append(':');
                output.Append('{');
            }
            else
            {
                output.Append('}');
            }
        }

        public void WriteJSONCollectionStart(StringBuilder output)
        {
            output.Append('"').Append(ElementStart).Append('"');
            output.Append(':');
            output.Append('[');
        }

        public void WriteJSONCollectionClose(StringBuilder output)
        {
            output.Append(']');
        }

        public void AppendJSONCollectionAttribute(StringBuilder output, object instance, bool isFirst)
        {
            if (instance != null)
            {
                if (!isFirst)
                {
                    output.Append(',');
                }

                ScalarType scalarType = this._scalarType;
                output.Append('"');
                scalarType.AppendValue(instance, output, false, Format.JSON);
                output.Append('"');
            }
        }

        public void WriteJSONPolymorphicCollectionStart(StringBuilder output)
        {
            output.Append('"').Append(_tagName).Append('"');
            output.Append(':');
            output.Append('[');
        }

        public void WriteJSONCloseTag(StringBuilder output)
        {
            output.Append('}');
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean ResolveUnresolvedScopeAnnotation()
        {
            if (unresolvedScopeAnnotation == null)
                return true;

            Boolean result = ResolveScopeAnnotation(unresolvedScopeAnnotation);
            if (result)
            {
                unresolvedScopeAnnotation = null;
                _declaringClassDescriptor.MapTagClassDescriptors(this);
            }
            return result;
        }

        private bool ResolveScopeAnnotation(string scopeAnnotation)
        {
            TranslationScope scope = TranslationScope.Get(scopeAnnotation);
            if (scope != null)
            {
                List<ClassDescriptor> scopeClassDescriptors = scope.GetClassDescriptors();
                InitTagClassDescriptorsArrayList(scopeClassDescriptors.Count);
                foreach (ClassDescriptor classDescriptor in scopeClassDescriptors)
                {
                    String tagName = classDescriptor.TagName;
                    _tagClassDescriptors.Add(tagName, classDescriptor);
                    _tagClasses.Add(tagName, classDescriptor.DescribedClass);
                }
            }
            return scope != null;
        }



        /// <summary>
        /// return an instance based on whether the graph switch is on or not
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="childClassDescriptor"></param>
        /// <param name="graphContext"></param>
        /// <returns></returns>
        private ElementState GetInstance(ecologylab.serialization.sax.Attributes attributes, ClassDescriptor childClassDescriptor,
			TranslationContext graphContext) 
	    {
		    ElementState result;

            if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON)
		    {
			    ElementState alreadyUnmarshalledObject = graphContext.GetFromMap(attributes);

			    if (alreadyUnmarshalledObject != null)
				    result = alreadyUnmarshalledObject;
			    else
				    result = childClassDescriptor.Instance;
		    }
		    else
		    {
			    result = childClassDescriptor.Instance;
		    }
		    return result;
	    }

        public ScalarType GetScalarType()
        {
            return _scalarType;
        }

        public Regex RegexPattern
        {
            get { return regex; }
            set { regex = value; }
        }

        public String RegexReplacement
        {
            get { return filterReplace; }
            set { filterReplace = value; }
        }

        public bool IsDefaultValueFromContext(object o)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(StreamWriter streamWriter, object o, object o1, Format xml)
        {
            throw new NotImplementedException();
        }

        public void AppendValue(StreamWriter streamWriter, object o, TranslationContext translationContext, Format xml)
        {
            throw new NotImplementedException();
        }

        public bool IsDefaultValue(string p)
        {
            throw new NotImplementedException();
        }

        public void AppendCollectionScalarValue(StreamWriter streamWriter, object o, TranslationContext translationContext, Format xml)
        {
            throw new NotImplementedException();
        }

        public object GetObject(object p0)
        {
            throw new NotImplementedException();
        }
    }
}
