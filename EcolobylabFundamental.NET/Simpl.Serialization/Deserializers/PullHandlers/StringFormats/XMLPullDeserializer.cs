﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Simpl.Serialization.Context;

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
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        public XmlPullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext, IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputTranslationScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        public XmlPullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext)
            : base(inputTranslationScope, inputContext)
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

            String rootTag = CurrentTagName;

            ClassDescriptor rootClassDescriptor = translationScope
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
            while(NextEvent() && _xmlReader.NodeType != XmlNodeType.EndElement )
            {
                FieldDescriptor currentFieldDescriptor = rootClassDescriptor.GetFieldDescriptorByTag(CurrentTagName);

                if(currentFieldDescriptor == null)
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
                }
            }
        }

        private void DeserializeComposite(object root, FieldDescriptor currentFieldDescriptor)
        {
            Object subRoot = GetSubRoot(currentFieldDescriptor, CurrentTagName, root);
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
            Boolean returnValue = false;
            while ((returnValue = _xmlReader.Read()) && (_xmlReader.NodeType != XmlNodeType.Element
                   && _xmlReader.NodeType != XmlNodeType.EndElement
                   && _xmlReader.NodeType != XmlNodeType.CDATA
                   && _xmlReader.NodeType != XmlNodeType.Text
                   ))
            {
            }

            return returnValue;
        }

        public string CurrentTagName
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