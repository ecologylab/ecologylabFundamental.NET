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
    /// XML specific deserializer, extends StringPullDeserializer and implements the parse method on input stream and input string. 
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
        /// Constructure to initialize loca data structures. 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        public XmlPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext)
            : base(inputSimplTypesScope, inputContext)
        {
        }

        /// <summary>
        /// overriding the method from the base class to implement Xml specific deserialization
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override object Parse(Stream stream)
        {
            ConfigureInput(stream);
            return Parse();
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
            ConfigureInput(new MemoryStream(Encoding.Default.GetBytes(inputString)));
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
                                                    + rootTag + ">; make sure if simplTypesScope is correct.");
            }

            root = rootClassDescriptor.GetInstance();

            DeserializationPreHook(root, translationContext);
            if (deserializationHookStrategy != null)
                deserializationHookStrategy.DeserializationPreHook(root, null);

            DeserializeAttributes(root, rootClassDescriptor);

            CreateObjectModel(root, rootClassDescriptor, rootTag);

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="rootClassDescriptor"></param>
        /// <param name="rootTag"></param>
        private void CreateObjectModel(object root, ClassDescriptor rootClassDescriptor, string rootTag)
        {
            while (NextEvent() && (_xmlReader.NodeType != XmlNodeType.EndElement || !CurrentTag.Equals(rootTag)))
            {
                if (_xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                FieldDescriptor currentFieldDescriptor = rootClassDescriptor.GetFieldDescriptorByTag(CurrentTag);

                if (currentFieldDescriptor == null)
                {
                    Console.WriteLine("ignoring tag " + CurrentTag);

                    if (!_xmlReader.IsEmptyElement)
                        SkipTag(CurrentTag);
                    continue;
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
                        DeserializeScalarCollectionElement(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CollectionElement:
                        DeserializeCompositeCollectionElement(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.MapElement:
                        DeserializeCompositeMapElement(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.Wrapper:
                        currentFieldDescriptor = currentFieldDescriptor.WrappedFd;
                        switch (currentFieldDescriptor.FdType)
                        {
                            case FieldTypes.CollectionScalar:
                               DeserializeScalarCollection(root, currentFieldDescriptor);
                                break;
                            case FieldTypes.CollectionElement:
                                DeserializeCompositeCollection(root, currentFieldDescriptor);
                                break;
                            case FieldTypes.MapElement:
                                DeserializeCompositeMap(root, currentFieldDescriptor);
                                break;
                            case FieldTypes.CompositeElement:
                                //TODO: wrapped composites
                                break;
                        }
                        break;
                    default:
                        NextEvent();
                        break;
                }
            }
        }

        private void SkipTag(string tag)
        {
            Stack<String> startElements = new Stack<string>();
            startElements.Push(tag);
            while (NextEvent())
            {
                switch (_xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (CurrentTag.Equals(tag))
                            startElements.Push(tag);
                        break;
                    case XmlNodeType.EndElement:
                        if (CurrentTag.Equals(tag))
                            startElements.Pop();
                        break;
                }
                if (startElements.Count <= 0)
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        private void DeserializeCompositeMapElement(object root, FieldDescriptor fd)
        {
            String compositeTagName = CurrentTag;
            Object subRoot = GetSubRoot(fd, compositeTagName, root);
            if (subRoot != null)
            {
                IDictionary dictionary = (IDictionary) fd.AutomaticLazyGetCollectionOrMap(root);
                Object key = ((IMappable) subRoot).Key();
                if (dictionary.Contains(key))
                {
                    //Note: overriding a key in map, duplicate data in xml. 
                    dictionary[key] = subRoot;
                }
                else
                {
                    dictionary.Add(key, subRoot);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        private void DeserializeCompositeCollectionElement(object root, FieldDescriptor fd)
        {
            String compositeTagName = CurrentTag;
            Object subRoot = GetSubRoot(fd, compositeTagName, root);
            if (subRoot != null)
            {
                IList collection = (IList) fd.AutomaticLazyGetCollectionOrMap(root);
                collection.Add(subRoot);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        private void DeserializeScalarCollectionElement(object root, FieldDescriptor fd)
        {
            if (NextEvent() && _xmlReader.NodeType == XmlNodeType.Text && _xmlReader.NodeType != XmlNodeType.EndElement)
            {
                String value = _xmlReader.ReadString();
                fd.AddLeafNodeToCollection(root, value, translationContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        private void DeserializeCompositeMap(object root, FieldDescriptor fd)
        {
            String currentTag = CurrentTag;
            while (NextEvent() && !(_xmlReader.NodeType == XmlNodeType.EndElement && CurrentTag.Equals(currentTag)))
            {
                DeserializeCompositeMapElement(root, fd);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        private void DeserializeCompositeCollection(object root, FieldDescriptor fd)
        {
            String currentTag = CurrentTag;
            while (NextEvent() && !(_xmlReader.NodeType == XmlNodeType.EndElement && CurrentTag.Equals(currentTag)))
            {
                DeserializeCompositeCollectionElement(root, fd);
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        private void DeserializeScalarCollection(object root, FieldDescriptor fd)
        {
            String currentTag = CurrentTag;
            while (NextEvent() && !(_xmlReader.NodeType == XmlNodeType.EndElement && CurrentTag.Equals(currentTag)))
            {
                DeserializeScalarCollectionElement(root, fd);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        private void DeserializeComposite(object root, FieldDescriptor currentFieldDescriptor)
        {
            Object subRoot = GetSubRoot(currentFieldDescriptor, CurrentTag, root);
            if (subRoot != null)
                currentFieldDescriptor.SetFieldToComposite(root, subRoot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFieldDescriptor"></param>
        /// <param name="currentTagName"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private object GetSubRoot(FieldDescriptor currentFieldDescriptor, string currentTagName, object root)
        {
            Object subRoot = null;
            ClassDescriptor subRootClassDescriptor = currentFieldDescriptor.ChildClassDescriptor(currentTagName);

            String simplReference;
            if ((simplReference = GetSimplReference()) != null)
            {
                subRoot = translationContext.GetFromMap(simplReference);
                return subRoot;
            }

            if(subRootClassDescriptor == null)
            {
                Debug.WriteLine("ignoring element: " + currentTagName);
                _xmlReader.Skip();
                return null;
            }

            subRoot = subRootClassDescriptor.GetInstance();
            DeserializeAttributes(subRoot, subRootClassDescriptor);

            if (!_xmlReader.IsEmptyElement)
                CreateObjectModel(subRoot, subRootClassDescriptor, currentTagName);
           
            return subRoot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetSimplReference()
        {
            String simplReference = null;
            while (_xmlReader.MoveToNextAttribute())
            {
                String tag = _xmlReader.Name;
                String value = _xmlReader.Value;

               if(TranslationContext.SimplRef.Equals(tag))
               {
                   simplReference = value;
               }
            }
            _xmlReader.MoveToElement();
            return simplReference;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        private void DeserializeScalar(object root, FieldDescriptor currentFieldDescriptor)
        {
            String value = _xmlReader.ReadString();
            currentFieldDescriptor.SetFieldToScalar(root, value, translationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="rootClassDescriptor"></param>
        /// <returns></returns>
        private Boolean DeserializeAttributes(object root, ClassDescriptor rootClassDescriptor)
        {
            while (_xmlReader.MoveToNextAttribute())
            {
                String tag = _xmlReader.Name;
                String value = _xmlReader.Value;

                //silently ignore simpl namespace. 
                if (tag.Equals(TranslationContext.SimplNamespace)) continue;

                if(tag.Equals(TranslationContext.SimplId))
                {
                    translationContext.MarkAsUnmarshalled(value, root);
                }
                else
                {
                    FieldDescriptor attributeFieldDescriptor = rootClassDescriptor.GetFieldDescriptorByTag(tag);
                    if (attributeFieldDescriptor != null)
                    {
                        attributeFieldDescriptor.SetFieldToScalar(root, value, translationContext);
                    }
                    else
                    {
                        Console.WriteLine("ignoring attribute: " + tag);
                    }   
                }
            }

            return _xmlReader.MoveToElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
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
