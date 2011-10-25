using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.Context;
using Simpl.Serialization.Types;

namespace Simpl.Serialization
{
    public class FieldDescriptor : DescriptorBase
    {
        public const String Null = ScalarType.DefaultValueString;

        [SimplScalar] protected FieldInfo field;


        [SimplComposite] private ClassDescriptor elementClassDescriptor;

        [SimplScalar] private String mapKeyFieldName;


        [SimplComposite] protected ClassDescriptor declaringClassDescriptor;

        [SimplScalar] private Type elementClass;

        [SimplScalar] private Boolean isGeneric;

        [SimplMap("polymorph_class_descriptor")] [SimplMapKeyField] private DictionaryList<String, ClassDescriptor>
            polymorphClassDescriptors;

        [SimplMap("polymorph_class")] private Dictionary<String, Type> polymorphClasses;

        [SimplMap("library_namespace")] private Dictionary<String, String> libraryNamespaces =
            new Dictionary<String, String>();

        [SimplScalar] private int type;

        [SimplScalar] private ScalarType scalarType;

        [SimplComposite] private CollectionType collectionType;

        [SimplScalar] private Hint xmlHint;

        [SimplScalar] private Boolean isEnum;


        private String[] dataFormat;

        [SimplScalar] private Boolean isCDATA;

        [SimplScalar] private Boolean needsEscaping;

        [SimplScalar] private Regex filterRegex;

        [SimplScalar] private String filterReplace;


        private FieldDescriptor wrappedFD;

        private Dictionary<Int32, ClassDescriptor> tlvClassDescriptors;

        [SimplScalar] private String unresolvedScopeAnnotation = null;


        [SimplScalar] private String collectionOrMapTagName;

        [SimplScalar] private String compositeTagName;


        [SimplScalar] private Boolean wrapped;

        private MethodInfo setValueMethod;

        private String bibtexTag = "";

        private Boolean isBibtexKey = false;

        [SimplScalar] private String fieldType;

        [SimplScalar] private String genericParametersString;

        private List<Type> dependencies = new List<Type>();

        public FieldDescriptor(ClassDescriptor baseClassDescriptor)
            : base(baseClassDescriptor.TagName, null)
        {

            declaringClassDescriptor = baseClassDescriptor;
            field = null;
            type = FieldTypes.Pseudo;
            ScalarType = null;
            bibtexTag = baseClassDescriptor.BibtexType;
        }


        public FieldDescriptor(ClassDescriptor baseClassDescriptor, FieldDescriptor wrappedFD,
                               String wrapperTag)
            : base(wrapperTag, null)
        {
            declaringClassDescriptor = baseClassDescriptor;
            this.wrappedFD = wrappedFD;
            type = FieldTypes.Wrapper;
        }

        public FieldDescriptor(ClassDescriptor declaringClassDescriptor, FieldInfo field, int annotationType)
            : base(XmlTools.GetXmlTagName(field), field.Name)
        {
            this.declaringClassDescriptor = declaringClassDescriptor;
            this.field = field;
            fieldType = field.FieldType.Name;
            if (XmlTools.IsAnnotationPresent(field, typeof (SimplMapKeyField)))
                mapKeyFieldName =
                    ((SimplMapKeyField) XmlTools.GetAnnotation(field, typeof (SimplMapKeyField))).FieldName;


            DerivePolymorphicDescriptors(field);

            type = FieldTypes.UnsetType;

            if (annotationType == FieldTypes.Scalar)
                type = DeriveScalarSerialization(field);
            else
                type = DeriveNestedSerialization(field, annotationType);

            String fieldName = field.Name;
            StringBuilder capFieldName = new StringBuilder(fieldName);
        }

        private int DeriveNestedSerialization(FieldInfo thatField, int annotationType)
        {
            int result = annotationType;
            Type thatFieldType = thatField.FieldType;

            switch (annotationType)
            {
                case FieldTypes.CompositeElement:
                    String compositeTag =
                        ((SimplComposite) XmlTools.GetAnnotation(thatField, typeof (SimplComposite))).TagName;
                    Boolean isWrap = XmlTools.IsAnnotationPresent(thatField, typeof (SimplWrap));

                    Boolean compositeTagIsNullOrEmpty = String.IsNullOrEmpty(compositeTag);

                    if (!IsPolymorphic)
                    {
                        if (isWrap && compositeTagIsNullOrEmpty)
                        {
                            String msg = "In " + declaringClassDescriptor.DescribedClass
                                         + "\n\tCan't translate  [SimplComposite] " + thatField.Name
                                         + " because its tag argument is missing.";

                            Debug.WriteLine(msg);
                            return FieldTypes.IgnoredAttribute;
                        }

                        elementClassDescriptor = ClassDescriptor.GetClassDescriptor(thatFieldType);
                        elementClass = elementClassDescriptor.DescribedClass;
                        compositeTag = XmlTools.GetXmlTagName(thatField);
                    }
                    else
                    {
                        if (!compositeTagIsNullOrEmpty)
                        {
                            String msg = "In " + declaringClassDescriptor.DescribedClass
                                         + "\n\tCan't translate  [SimplComposite] " + thatField.Name
                                         + " because its tag argument is missing.";

                            Debug.WriteLine(msg);
                        }
                    }
                    compositeTagName = compositeTag;
                    break;
                case FieldTypes.CollectionElement:
                    if (!(typeof(IList).IsAssignableFrom(thatField.FieldType)))
                    {
                        String msg = "In " + declaringClassDescriptor.DescribedClass + "\n\tCan't translate  "
                                     + "[SimplCollection] " + field.Name
                                     + " because the annotated field is not an instance of " +
                                     typeof (IList).Name
                                     + ".";

                        Debug.WriteLine(msg);
                        return FieldTypes.IgnoredAttribute;
                    }


                    String collectionTag =
                        ((SimplCollection) XmlTools.GetAnnotation(thatField, typeof (SimplCollection))).TagName;

                    if (!IsPolymorphic)
                    {
                        Type collectionElementType = GetTypeArgs(thatField, 0);
                        if (String.IsNullOrEmpty(collectionTag))
                        {
                            String msg = "In " + declaringClassDescriptor.DescribedClass
                                         + "\n\tCan't translate [SimplCollection]" + field.Name
                                         + " because its tag argument is missing.";
                            Debug.WriteLine(msg);
                            return FieldTypes.IgnoredElement;
                        }
                        if (collectionElementType == null)
                        {
                            String msg = "In " + declaringClassDescriptor.DescribedClass
                                         + "\n\tCan't translate [SimplCollection] " + field.Name
                                         + " because the parameterized type argument for the Collection is missing.";
                            Debug.WriteLine(msg);
                            return FieldTypes.IgnoredElement;
                        }
                        if (!TypeRegistry.ContainsScalarType(collectionElementType))
                        {
                            elementClassDescriptor = ClassDescriptor.GetClassDescriptor(collectionElementType);
                            elementClass = elementClassDescriptor.DescribedClass;
                        }
                        else
                        {
                            result = FieldTypes.CollectionScalar;
                            DeriveScalarSerialization(collectionElementType, field);
                            if (ScalarType == null)
                            {
                                result = FieldTypes.IgnoredElement;
                                String msg = "Can't identify ScalarType for serialization of " + collectionElementType;
                                Debug.WriteLine(msg);
                            }
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(collectionTag))
                        {
                            String msg = "In " + declaringClassDescriptor.DescribedClass
                                         + "\n\tIgnoring argument to  [SimplCollection] " + field.Name
                                         + " because it is declared polymorphic with [SimplClasses].";
                        }
                    }
                    collectionOrMapTagName = collectionTag;
                    collectionType = TypeRegistry.GetCollectionType(thatField);
                    break;

                case FieldTypes.MapElement:
                    if (!(typeof(IDictionary).IsAssignableFrom(thatField.FieldType)))
                    {
                        String msg = "In " + declaringClassDescriptor.DescribedClass + "\n\tCan't translate  "
                                     + "[SimplMap] " + field.Name
                                     + " because the annotated field is not an instance of " +
                                     typeof (IDictionary).Name
                                     + ".";

                        Debug.WriteLine(msg);
                        return FieldTypes.IgnoredAttribute;
                    }


                    String mapTag =
                        ((SimplMap) XmlTools.GetAnnotation(thatField, typeof (SimplMap))).TagName;

                    if (!IsPolymorphic)
                    {
                        Type mapElementType = GetTypeArgs(thatField, 1);
                        if (String.IsNullOrEmpty(mapTag))
                        {
                            String msg = "In " + declaringClassDescriptor.DescribedClass
                                         + "\n\tCan't translate [SimplMap]" + field.Name
                                         + " because its tag argument is missing.";
                            Debug.WriteLine(msg);
                            return FieldTypes.IgnoredElement;
                        }
                        if (mapElementType == null)
                        {
                            String msg = "In " + declaringClassDescriptor.DescribedClass
                                         + "\n\tCan't translate [SimplMap] " + field.Name
                                         + " because the parameterized type argument for the map is missing.";
                            Debug.WriteLine(msg);
                            return FieldTypes.IgnoredElement;
                        }
                        if (!TypeRegistry.ContainsScalarType(mapElementType))
                        {
                            elementClassDescriptor = ClassDescriptor.GetClassDescriptor(mapElementType);
                            elementClass = elementClassDescriptor.DescribedClass;
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(mapTag))
                        {
                            String msg = "In " + declaringClassDescriptor.DescribedClass
                                         + "\n\tIgnoring argument to  [SimplMap] " + field.Name
                                         + " because it is declared polymorphic with [SimplClasses].";
                        }
                    }
                    collectionOrMapTagName = mapTag;
                    collectionType = TypeRegistry.GetCollectionType(thatField);
                    break;
            }

            switch (annotationType)
            {
                case FieldTypes.CollectionElement:
                case FieldTypes.MapElement:
                    if (!XmlTools.IsAnnotationPresent(thatField, typeof (SimplNoWrap)))
                        wrapped = true;
                    collectionType = TypeRegistry.GetCollectionType(thatField);
                    break;
                case FieldTypes.CompositeElement:
                    if (XmlTools.IsAnnotationPresent(thatField, typeof (SimplWrap)))
                    {
                        wrapped = true;
                    }
                    break;
            }

            return result;
        }

        private Type GetTypeArgs(FieldInfo thatField, int p)
        {
            Type result;
            Type t = thatField.FieldType;

            Type[] typeArgs = t.GetGenericArguments();

            {
                int max = typeArgs.Length - 1;
                if (p > max)
                {
                    p = max;
                }

                result = typeArgs[p];
            }

            return result;
        }

        public bool IsPolymorphic
        {
            get { return (polymorphClassDescriptors != null) || (unresolvedScopeAnnotation != null); }
        }

        private int DeriveScalarSerialization(FieldInfo scalarField)
        {
            int result = DeriveScalarSerialization(scalarField.FieldType, scalarField);
            if (xmlHint == Hint.XmlText || xmlHint == Hint.XmlTextCdata)
                declaringClassDescriptor.ScalarTextFD = this;
            return result;
        }

        private int DeriveScalarSerialization(Type thatType, FieldInfo scalarField)
        {
            isEnum = XmlTools.IsEnum(scalarField);
            xmlHint = XmlTools.SimplHint(scalarField);
            ScalarType = TypeRegistry.GetScalarType(thatType);

            if (ScalarType == null)
            {
                String msg = "Can't find ScalarType to serialize field: \t\t" + thatType.Name
                             + "\t" + scalarField.Name + ";";

                Debug.WriteLine(msg);
                return (xmlHint == Hint.XmlAttribute) ? FieldTypes.IgnoredAttribute : FieldTypes.IgnoredElement;
            }

            dataFormat = XmlTools.GetFormatAnnotation(field);

            if (xmlHint != Hint.XmlAttribute)
            {
                needsEscaping = ScalarType.NeedsEscaping;
                isCDATA = xmlHint == Hint.XmlLeafCdata || xmlHint == Hint.XmlTextCdata;
            }
            return FieldTypes.Scalar;
        }

        private void DerivePolymorphicDescriptors(FieldInfo pField)
        {
            SimplScope scopeAttribute = (SimplScope) XmlTools.GetAnnotation(pField, typeof (SimplScope));
            String scopeAttributeValue = scopeAttribute == null ? null : scopeAttribute.TranslationScope;

            if (!String.IsNullOrEmpty(scopeAttributeValue))
            {
                if (!ResolveScopeAttribute(scopeAttributeValue))
                {
                    unresolvedScopeAnnotation = scopeAttributeValue;
                    declaringClassDescriptor.RegisterUnresolvedScopeAnnotationFD(this);
                }
            }

            SimplClasses classesAttribute = (SimplClasses) XmlTools.GetAnnotation(pField, typeof (SimplClasses));
            Type[] classesAttributeValue = classesAttribute == null ? null : classesAttribute.Classes;

            if ((classesAttribute != null) && classesAttributeValue.Length > 0)
            {
                InitPolymorphicClassDescriptorsList(classesAttributeValue.Length);
                foreach (Type thatType in classesAttributeValue)
                {
                    ClassDescriptor classDescriptor = ClassDescriptor.GetClassDescriptor(thatType);
                    RegisterPolymorphicDescriptor(classDescriptor);
                    polymorphClasses.Put(classDescriptor.TagName, classDescriptor.DescribedClass);
                }
            }
        }

        private void RegisterPolymorphicDescriptor(ClassDescriptor classDescriptor)
        {
            if (polymorphClassDescriptors == null)
                InitPolymorphicClassDescriptorsList(1);

            String classTag = classDescriptor.TagName;
            polymorphClassDescriptors.Put(classTag, classDescriptor);
            tlvClassDescriptors.Put(classTag.GetHashCode(), classDescriptor);

            if (otherTags != null)
                foreach (String otherTag in classDescriptor.OtherTags)
                {
                    if (!String.IsNullOrEmpty(otherTag))
                    {
                        polymorphClassDescriptors.Put(otherTag, classDescriptor);
                        tlvClassDescriptors.Put(otherTag.GetHashCode(), classDescriptor);
                    }
                }
        }

        private void InitPolymorphicClassDescriptorsList(Int32 size)
        {
            if (polymorphClassDescriptors == null)
                polymorphClassDescriptors = new DictionaryList<String, ClassDescriptor>(size);
            if (polymorphClasses == null)
                polymorphClasses = new Dictionary<String, Type>(size);
            if (tlvClassDescriptors == null)
                tlvClassDescriptors = new Dictionary<Int32, ClassDescriptor>(size);
        }

        private bool ResolveScopeAttribute(string scopeAttributeValue)
        {
            SimplTypesScope scope = SimplTypesScope.Get(scopeAttributeValue);
            if (scope != null)
            {
                List<ClassDescriptor> scopeClassDescriptors = scope.ClassDescriptors;
                InitPolymorphicClassDescriptorsList(scopeClassDescriptors.Count);
                foreach (var scopeClassDescriptor in scopeClassDescriptors)
                {
                    polymorphClassDescriptors.Put(scopeClassDescriptor.TagName, scopeClassDescriptor);
                    polymorphClasses.Put(scopeClassDescriptor.TagName, scopeClassDescriptor.DescribedClass);
                    tlvClassDescriptors.Put(tagName.GetHashCode(), scopeClassDescriptor);
                }
            }

            return scope != null;
        }

        public String UnresolvedScopeAnnotation
        {
            get { return unresolvedScopeAnnotation; }
            set { unresolvedScopeAnnotation = value; }
        }

        public override string JavaTypeName
        {
            get { return elementClassDescriptor != null ? elementClassDescriptor.JavaTypeName : ScalarType.JavaTypeName; }
        }

        public override string CSharpTypeName
        {
            get { return elementClassDescriptor != null ? elementClassDescriptor.CSharpTypeName : ScalarType.CSharpTypeName; }
        }

        public override string ObjectiveCTypeName
        {
            get
            {
                return elementClassDescriptor != null
                           ? elementClassDescriptor.ObjectiveCTypeName
                           : ScalarType.ObjectiveCTypeName;
            }
        }

        public override string DbTypeName
        {
            get { return elementClassDescriptor != null ? elementClassDescriptor.DbTypeName : ScalarType.DbTypeName; }
        }

        public override List<string> OtherTags
        {
            get { throw new NotImplementedException(); }
        }

        public Int32 FdType
        {
            get { return type; }
        }

        public Hint XmlHint
        {
            get { return xmlHint; }
            set { xmlHint = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMarshallOnly
        {
            get { return ScalarType != null && ScalarType.IsMarshallOnly; }
        }

        public FieldInfo Field
        {
            get { return field; }
        }

        public Boolean IsWrapped
        {
            get { return wrapped; }
        }

        public bool IsCollection
        {
            get
            {
                switch (type)
                {
                    case FieldTypes.MapElement:
                    case FieldTypes.CollectionElement:
                    case FieldTypes.CollectionScalar:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public String CollectionOrMapTagName
        {
            get { return collectionOrMapTagName; }
        }

        public Boolean ResolveUnresolvedScopeAnnotation()
        {
            if (unresolvedScopeAnnotation == null)
                return true;

            Boolean result = ResolveScopeAnnotation(unresolvedScopeAnnotation);
            if (result)
            {
                unresolvedScopeAnnotation = null;
                declaringClassDescriptor.MapTagClassDescriptors(this);
            }
            return result;
        }

        private bool ResolveScopeAnnotation(string scopeAnnotation)
        {
            SimplTypesScope scope = SimplTypesScope.Get(scopeAnnotation);
            if (scope != null)
            {
                List<ClassDescriptor> scopeClassDescriptors = scope.GetClassDescriptors();
                InitTagClassDescriptorsArrayList(scopeClassDescriptors.Count);
                foreach (ClassDescriptor classDescriptor in scopeClassDescriptors)
                {
                    String tagName = classDescriptor.TagName;
                    polymorphClassDescriptors.Put(tagName, classDescriptor);
                    polymorphClasses.Put(tagName, classDescriptor.DescribedClass);
                }
            }
            return scope != null;
        }

        private void InitTagClassDescriptorsArrayList(int initialSize)
        {
            if (polymorphClassDescriptors == null)
            {
                polymorphClassDescriptors = new DictionaryList<string, ClassDescriptor>(initialSize);
            }
            if (polymorphClasses == null)
            {
                polymorphClasses = new Dictionary<String, Type>(initialSize);
            }
        }

        public DictionaryList<String, ClassDescriptor> PolymorphClassDescriptors
        {
            get { return polymorphClassDescriptors; }
        }

        public Boolean IsCdata
        {
            get { return isCDATA; }
        }

        public String ElementStart
        {
            get { return IsCollection ? collectionOrMapTagName : IsNested ? compositeTagName : tagName; }
        }

        public Boolean IsNested
        {
            get { return type == FieldTypes.CompositeElement; }
        }

        public FieldDescriptor WrappedFd
        {
            get { return wrappedFD; }
        }

        public ScalarType ScalarType
        {
            get { return scalarType; }
            set { scalarType = value; }
        }

        public Regex FilterRegex
        {
            get { return filterRegex; }
            set { filterRegex = value; }
        }

        public string FilterReplace
        {
            get { return filterReplace; }
            set { filterReplace = value; }
        }

        public object BibtexTagName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IsBibtexKey
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int TlvId
        {
            get { return ElementStart.GetTlvId(); }
        }

        public int WrappedTLVId
        {
            get
            {
                int tempTlvId = 0;

                if (tagName != null)
                    tempTlvId = tagName.GetTlvId();

                return tempTlvId; ;
            }
        }


        public void AppendValue(TextWriter textWriter, object obj, TranslationContext translationContext, Format format)
        {
            ScalarType.AppendValue(textWriter, this, obj, format);
        }

        public bool IsDefaultValueFromContext(object context)
        {
            if (context != null)
            {
                return ScalarType.IsDefaultValue(field, context);
            }

            return false;
        }

        public bool IsDefaultValue(String value)
        {
            return ScalarType.IsDefaultValue(value);
        }

        public void AppendCollectionScalarValue(TextWriter textWriter, object obj, TranslationContext translationContext,
                                                Format format)
        {
            ScalarType.AppendValue(obj, textWriter, !IsCdata, format);
        }

        public object GetObject(object obj)
        {
            return field.GetValue(obj);
        }

        public void SetFieldToScalar(object root, string value, TranslationContext translationContext)
        {
            if (ScalarType != null && !ScalarType.IsMarshallOnly)
            {
                ScalarType.SetField(root, field, value, dataFormat, translationContext);
            }
        }

        public void SetFieldToComposite(object root, object subRoot)
        {
            field.SetValue(root, subRoot);
        }

        public ClassDescriptor ChildClassDescriptor(string currentTagName)
        {
            if (!IsPolymorphic)
                return elementClassDescriptor;

            if(polymorphClassDescriptors == null)
            {
                Debug.WriteLine("The field is declared polymorphic, but its polymorphic ClassDescriptor don't exist! Check annoation and is simplTypesScopes defined?");
                return null;
            }

            if (polymorphClassDescriptors.ContainsKey(currentTagName))
                return polymorphClassDescriptors[currentTagName];

            return null;
        }

        public void AddLeafNodeToCollection(object root, string value, TranslationContext translationContext)
        {
            if(String.IsNullOrEmpty(value))
            {
                return;
            }

            if(ScalarType != null)
            {
                Object typeConvertedValue = ScalarType.GetInstance(value, dataFormat, translationContext);

                if(typeConvertedValue != null)
                {
                    IList collection = (IList) AutomaticLazyGetCollectionOrMap(root);
                    collection.Add(typeConvertedValue);
                }
            }
        }

        public Object AutomaticLazyGetCollectionOrMap(object root)
        {
            Object
            collection = null;

            collection = field.GetValue(root);
            if(collection == null)
            {
                collection = Activator.CreateInstance(Field.FieldType); //TODO: use collectionType.Instance but first have generic type parameter specification system in S.IM.PL
                field.SetValue(root, collection);
            }

            return collection;
        }

        public bool IsCollectionTag(string currentTag)
        {
            return IsPolymorphic
                       ? polymorphClassDescriptors.ContainsKey(currentTag)
                       : collectionOrMapTagName.Equals(currentTag);
        }

        public ClassDescriptor GetChildClassDescriptor(int tlvType)
        {
            throw new NotImplementedException();
        }
    }
}
