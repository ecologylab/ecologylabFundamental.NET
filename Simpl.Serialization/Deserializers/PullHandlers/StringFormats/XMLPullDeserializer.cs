using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Simpl.Serialization.Context;
using Simpl.Serialization.Types.Element;

namespace Simpl.Serialization.Deserializers.PullHandlers.StringFormats
{
    /// <summary>
    /// 
    /// </summary>
    class XmlPullDeserializer : StringPullDeserializer
    {
        /// <summary>
        /// 
        /// </summary>
        private XmlTextReader _xmlReader; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        public XmlPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext, IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputSimplTypesScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        public XmlPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext)
            : base(inputSimplTypesScope, inputContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public override Object Parse(String inputString)
        {
            ConfigureInput(inputString);
            return Parse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        private void ConfigureInput(String inputString)
        {
            ConfigureInput(new MemoryStream(Encoding.ASCII.GetBytes(inputString)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        private void ConfigureInput(Stream inputStream)
        {
            _xmlReader = new XmlTextReader(inputStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private object Parse()
        {
            Object root = null;

            NextEvent();

            if (_xmlReader.NodeType != XmlNodeType.Element)
            {
                throw new SimplTranslationException("start of an element expected");
            }

            String rootTag = CurrentTag;

            ClassDescriptor rootClassDescriptor = simplTypesScope
                .GetClassDescriptorByTag(rootTag);

            if (rootClassDescriptor == null)
            {
                throw new SimplTranslationException("cannot find the class descriptor for root element <"
                                                    + rootTag + ">; make sure if translation scope is correct.");
            }

            root = rootClassDescriptor.GetInstance();

            DeserializationPreHook(root, translationContext);
            if (deserializationHookStrategy != null)
                deserializationHookStrategy.DeserializationPreHook(root, null);

            DeserializeAttributes(root, rootClassDescriptor);

            CreateObjectModel(root, rootClassDescriptor, rootTag);

            return root;
        }

        private void CreateObjectModel(object root, ClassDescriptor rootClassDescriptor, string rootTag)
        {
            while (NextEvent() && _xmlReader.NodeType != XmlNodeType.EndElement && !CurrentTag.Equals(rootTag))
            {
                if (_xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                FieldDescriptor currentFieldDescriptor = rootClassDescriptor.GetFieldDescriptorByTag(CurrentTag);

                if (currentFieldDescriptor == null)
                {
                    break;
                }

                switch (currentFieldDescriptor.FdType)
                {
                    case FieldTypes.Scalar:
                        DeserializeScalar(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CompositeElement:
                        DeserializeComposite(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CollectionScalar:
                        DeserializeScalarCollection(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CollectionElement:
                        DeserializeCompositeCollection(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.MapElement:
                        DeserializeCompositeMap(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.Wrapper:
                        NextEvent();
                        currentFieldDescriptor = currentFieldDescriptor.WrappedFd;
                        if (currentFieldDescriptor.FdType == FieldTypes.CompositeElement)
                            goto case FieldTypes.CompositeElement;
                        if (currentFieldDescriptor.FdType == FieldTypes.CollectionScalar)
                            goto case FieldTypes.CollectionScalar;
                        if (currentFieldDescriptor.FdType == FieldTypes.CollectionElement)
                            goto case FieldTypes.CollectionElement;
                        break;
                    case FieldTypes.IgnoredElement:
                        _xmlReader.Skip();
                        break;
                    default:
                        NextEvent();
                        break;
                }
            }
        }

        private void DeserializeCompositeMap(object root, FieldDescriptor fd)
        {
            while (fd.IsCollectionTag(CurrentTag))
            {
                String compositeTagName = CurrentTag;
                Object subRoot = GetSubRoot(fd, compositeTagName, root);
                if (subRoot is IMappable)
                {
                    Object key = ((IMappable) subRoot).Key();
                    IDictionary dictionary = (IDictionary) fd.AutomaticLazyGetCollectionOrMap(root);
                    dictionary.Add(key, subRoot);
                }
                NextEvent();
            }
        }

        private void DeserializeCompositeCollection(object root, FieldDescriptor fd)
        {
            while(fd.IsCollectionTag(CurrentTag))
            {
                String compositeTagName = CurrentTag;
                Object subRoot = GetSubRoot(fd, compositeTagName, root);
                IList collection = (IList) fd.AutomaticLazyGetCollectionOrMap(root);
                collection.Add(subRoot);
                if (_xmlReader.IsEmptyElement) NextEvent();
            }
        }

        private void DeserializeScalarCollection(object root, FieldDescriptor fd)
        {
            
            while(fd.IsCollectionTag(CurrentTag))
            {
                if(NextEvent() && _xmlReader.NodeType == XmlNodeType.Text && _xmlReader.NodeType != XmlNodeType.EndElement)
                {
                    String value = _xmlReader.ReadString();
                    fd.AddLeafNodeToCollection(root, value, translationContext);
                }
            }
        }

        private void DeserializeComposite(object root, FieldDescriptor currentFieldDescriptor)
        {
            Object subRoot = GetSubRoot(currentFieldDescriptor, CurrentTag, root);
            currentFieldDescriptor.SetFieldToComposite(root, subRoot);
        }

        private object GetSubRoot(FieldDescriptor currentFieldDescriptor, string currentTagName, object root)
        {
            Object subRoot = null;
            ClassDescriptor subRootClassDescriptor = currentFieldDescriptor.ChildClassDescriptor(currentTagName);

            subRoot = subRootClassDescriptor.GetInstance();
            DeserializeAttributes(subRoot, subRootClassDescriptor);

            if (!_xmlReader.IsEmptyElement)
                CreateObjectModel(subRoot, subRootClassDescriptor, currentTagName);
           
            return subRoot;
        }

        private void DeserializeScalar(object root, FieldDescriptor currentFieldDescriptor)
        {
            String value = _xmlReader.ReadString();
            currentFieldDescriptor.SetFieldToScalar(root, value, translationContext);
        }

        private Boolean DeserializeAttributes(object root, ClassDescriptor rootClassDescriptor)
        {
            while (_xmlReader.MoveToNextAttribute())
            {
                String tag = _xmlReader.Name;
                String value = _xmlReader.Value;

                FieldDescriptor attributeFieldDescriptor = rootClassDescriptor.GetFieldDescriptorByTag(tag);
                if (attributeFieldDescriptor != null)
                {
                    attributeFieldDescriptor.SetFieldToScalar(root, value, translationContext);
                }
                else
                {
                    Debug.WriteLine("ignoring attribute: " + tag);
                }
            }

            return _xmlReader.MoveToElement();
        }

        private Boolean NextEvent()
        {
            Boolean returnValue;
            while ((returnValue = _xmlReader.Read()) && (_xmlReader.NodeType != XmlNodeType.Element
                   && _xmlReader.NodeType != XmlNodeType.EndElement
                   && _xmlReader.NodeType != XmlNodeType.CDATA
                   && _xmlReader.NodeType != XmlNodeType.Text
                   ))
            {
            }

            return returnValue;
        }

        public string CurrentTag
        {
            get
            {
                if (_xmlReader.Prefix.Length != 0)
                    return _xmlReader.Prefix + ":" + _xmlReader.LocalName;

                return _xmlReader.LocalName;
            }
        }
    }
}
