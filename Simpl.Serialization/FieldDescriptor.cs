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

        [SimplScalar] 
        protected FieldInfo field;


        [SimplComposite] 
        private ClassDescriptor elementClassDescriptor;

        [SimplScalar] 
        private String mapKeyFieldName;


        [SimplComposite] 
        protected ClassDescriptor declaringClassDescriptor;

        [SimplScalar] 
        private Type elementClass;

        [SimplScalar] 
        private Boolean isGeneric;

        [SimplMap("polymorph_class_descriptor")]
        [SimplMapKeyField] 
        private DictionaryList<String, ClassDescriptor> polymorphClassDescriptors;

        [SimplMap("polymorph_class")] 
        private Dictionary<String, Type> polymorphClasses;

        [SimplMap("library_namespace")] 
        private Dictionary<String, String> libraryNamespaces = new Dictionary<String, String>();

        [SimplScalar] 
        private int type;

        [SimplScalar]
        private ScalarType scalarType;

        [SimplComposite]
        private CollectionType collectionType;

        [SimplScalar]
        private Hint xmlHint;

        [SimplScalar]
        private Boolean isEnum;


        private String[] format;

        [SimplScalar]
        private Boolean isCDATA;

        [SimplScalar]
        private Boolean needsEscaping;

        [SimplScalar]
        private Regex filterRegex;

        [SimplScalar]
        private String filterReplace;


        private FieldDescriptor wrappedFD;

        private Dictionary<Int32, ClassDescriptor> tlvClassDescriptors;

        [SimplScalar]
        private String unresolvedScopeAnnotation = null;


        [SimplScalar]
        private String collectionOrMapTagName;

        [SimplScalar]
        private String compositeTagName;


        [SimplScalar]
        private Boolean wrapped;

        private MethodInfo setValueMethod;

        private String bibtexTag = "";

        private Boolean isBibtexKey = false;

        [SimplScalar]
        private String fieldType;

        [SimplScalar]
        private String genericParametersString;

        private List<Type> dependencies = new List<Type>();

        public FieldDescriptor(ClassDescriptor baseClassDescriptor)
            : base(baseClassDescriptor.TagName, null)
        {

            declaringClassDescriptor = baseClassDescriptor;
            field = null;
            type = FieldTypes.Pseudo;
            scalarType = null;
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
                    if (!(thatField is IList))
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
                            if (scalarType == null)
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
                    if (!(thatField is IDictionary))
                    {
                        String msg = "In " + declaringClassDescriptor.DescribedClass + "\n\tCan't translate  "
                                     + "[SimplMap] " + field.Name
                                     + " because the annotated field is not an instance of " +
                                     typeof (IList).Name
                                     + ".";

                        Debug.WriteLine(msg);
                        return FieldTypes.IgnoredAttribute;
                    }


                    String mapTag =
                        ((SimplMap) XmlTools.GetAnnotation(thatField, typeof (SimplMap))).TagName;

                    if (!IsPolymorphic)
                    {
                        Type mapElementType = GetTypeArgs(thatField, 0);
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

            switch(annotationType)
            {
                case FieldTypes.CollectionElement:
                case FieldTypes.MapElement:
                    if (!XmlTools.IsAnnotationPresent(thatField, typeof(SimplNoWrap)))
                        wrapped = true;
                    collectionType = TypeRegistry.GetCollectionType(thatField);
                    break;
                case FieldTypes.CompositeElement: 
                    if(XmlTools.IsAnnotationPresent(thatField, typeof(SimplWrap)))
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
            scalarType = TypeRegistry.GetScalarType(thatType);

            if(scalarType == null)
            {
                String msg = "Can't find ScalarType to serialize field: \t\t" + thatType.Name
                    + "\t" + scalarField.Name + ";";

                Debug.WriteLine(msg);
                return (xmlHint == Hint.XmlAttribute) ? FieldTypes.IgnoredAttribute : FieldTypes.IgnoredElement;
            }

            format = XmlTools.GetFormatAnnotation(field);

            if(xmlHint != Hint.XmlAttribute)
            {
                needsEscaping = scalarType.NeedsEscaping;
                isCDATA = xmlHint == Hint.XmlLeafCdata || xmlHint == Hint.XmlTextCdata;
            }
            return FieldTypes.Scalar;
        }

        private void DerivePolymorphicDescriptors(FieldInfo pField)
        {
            SimplScope scopeAttribute = (SimplScope) XmlTools.GetAnnotation(pField, typeof (SimplScope));
            String scopeAttributeValue = scopeAttribute == null ? null : scopeAttribute.TranslationScope;

            if(!String.IsNullOrEmpty(scopeAttributeValue))
            {
                if(!ResolveScopeAttribute(scopeAttributeValue))
                {
                    unresolvedScopeAnnotation = scopeAttributeValue;
                    declaringClassDescriptor.RegisterUnresolvedScopeAnnotationFD(this);
                }
            }

            SimplClasses classesAttribute = (SimplClasses) XmlTools.GetAnnotation(pField, typeof(SimplClasses));
            Type[] classesAttributeValue = classesAttribute == null ? null : classesAttribute.Classes;

            if((classesAttribute != null) && classesAttributeValue.Length > 0)
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
            TranslationScope scope = TranslationScope.Get(scopeAttributeValue);
            if(scope != null)
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
            get { return elementClassDescriptor != null ? elementClassDescriptor.JavaTypeName : scalarType.JavaTypeName; }
        }

        public override string CSharpTypeName
        {
            get { return elementClassDescriptor != null ? elementClassDescriptor.CSharpTypeName : scalarType.CSharpTypeName; }
        }

        public override string ObjectiveCTypeName
        {
            get { return elementClassDescriptor != null ? elementClassDescriptor.ObjectiveCTypeName : scalarType.ObjectiveCTypeName; }
        }

        public override string DbTypeName
        {
            get { return elementClassDescriptor != null ? elementClassDescriptor.DbTypeName : scalarType.DbTypeName; }
        }

        public override List<string> OtherTags
        {
            get { throw new NotImplementedException(); }
        }

        public Int32 FdType { get { return type; } }

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

        public FieldInfo Field
        {
            get
            {
                return field;
            }
        }

        public Boolean IsWrapped
        {
            get
            {
                return wrapped;
            }
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
            get
            {
                return collectionOrMapTagName;
            }
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
            TranslationScope scope = TranslationScope.Get(scopeAnnotation);
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

        public bool IsDefaultValue(String value)
        {
            return scalarType.IsDefaultValue(value);
        }

        public void AppendValue(TextWriter textWriter, object obj, TranslationContext translationContext, Format format)
        {
            scalarType.AppendValue(textWriter, this, obj, format);
        }

        public bool IsDefaultValueFromContext(object context)
        {
            if (context != null)
            {
                return scalarType.IsDefaultValue(field, context);
            }

            return false;
        }

        public void AppendCollectionScalarValue(TextWriter streamWriter, object o, TranslationContext translationContext, Format xml)
        {
            throw new NotImplementedException();
        }

        public object GetObject(object obj)
        {
            return field.GetValue(obj);
        }
    }
}
