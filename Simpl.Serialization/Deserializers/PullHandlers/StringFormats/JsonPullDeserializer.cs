using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using LitJson;
using Simpl.Serialization.Context;
using Simpl.Serialization.Types.Element;

namespace Simpl.Serialization.Deserializers.PullHandlers.StringFormats
{
    public class JsonPullDeserializer : StringPullDeserializer
    {
        private JsonReader _jsonReader;

        private String test;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        public JsonPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext,
                                    IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputSimplTypesScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        public JsonPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext)
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
            test = inputString;
            ConfigureInput(new MemoryStream(Encoding.ASCII.GetBytes(inputString)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        private void ConfigureInput(Stream inputStream)
        {
            _jsonReader = new JsonReader(new StreamReader(inputStream));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private object Parse()
        {
            _jsonReader.Read();
            if (_jsonReader.Token != JsonToken.ObjectStart)
            {
                Debug.WriteLine("JSON Translation ERROR: not a valid JSON object. It should start with {");
            }

            // move the first field in the document. typically it is the root element.
            _jsonReader.Read();

            Object root = null;

            // find the classdescriptor for the root element.
            ClassDescriptor rootClassDescriptor = simplTypesScope.GetClassDescriptorByTag(_jsonReader.Value.ToString());

            root = rootClassDescriptor.GetInstance();

            // move to the first field of the root element.
            _jsonReader.Read();
            _jsonReader.Read();

            // complete the object model from root and recursively of the fields it is composed of
            CreateObjectModel(root, rootClassDescriptor);

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="rootClassDescriptor"></param>
        private void CreateObjectModel(object root, ClassDescriptor rootClassDescriptor)
        {
            while (_jsonReader.Token != JsonToken.ObjectEnd)
            {
                object jsonReaderValue = _jsonReader.Value;

                FieldDescriptor currentFieldDescriptor =
                    rootClassDescriptor.GetFieldDescriptorByTag(jsonReaderValue.ToString());

                if(currentFieldDescriptor == null)
                {
                    String ignoredTag = jsonReaderValue.ToString();
                    IgnoreCurrentTag();
                    Console.WriteLine("WARNING: ignoring tag " + ignoredTag);
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
                        DeserializeScalarCollection(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CollectionElement:
                        DeserializeCompositeCollection(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.MapElement:
                        DeserializeCompositeMap(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.Wrapper:
                        if (!currentFieldDescriptor.WrappedFd.IsPolymorphic)
                            _jsonReader.Read();

                        _jsonReader.Read();

                        currentFieldDescriptor = currentFieldDescriptor.WrappedFd;
                        if (currentFieldDescriptor.FdType == FieldTypes.CompositeElement)
                            goto case FieldTypes.CompositeElement;
                        if (currentFieldDescriptor.FdType == FieldTypes.CollectionScalar)
                            goto case FieldTypes.CollectionScalar;
                        if (currentFieldDescriptor.FdType == FieldTypes.CollectionElement)
                            goto case FieldTypes.CollectionElement;
                        if (currentFieldDescriptor.FdType == FieldTypes.MapElement)
                            goto case FieldTypes.MapElement;

                        break;
                }
                _jsonReader.Read();
            }
        }

        private void IgnoreCurrentTag()
        {
            _jsonReader.Read();

            switch (_jsonReader.Token)
            {
                case JsonToken.ObjectStart:
                    Stack<JsonToken> objectStarts = new Stack<JsonToken>();
                    objectStarts.Push(JsonToken.ObjectStart);
                    while (_jsonReader.Read())
                    {
                        if (_jsonReader.Token == JsonToken.ObjectEnd)
                        {
                            objectStarts.Pop();
                            if (objectStarts.Count <= 0)
                            {
                                _jsonReader.Read();
                                break;
                            }

                        }
                        if (_jsonReader.Token == JsonToken.ObjectStart)
                        {
                            objectStarts.Push(JsonToken.ObjectStart);
                        }
                    }
                    break;
                case JsonToken.ArrayStart:
                    Stack<JsonToken> arrayStart = new Stack<JsonToken>();
                    arrayStart.Push(JsonToken.ArrayStart);
                    while (_jsonReader.Read())
                    {
                        if (_jsonReader.Token == JsonToken.ArrayEnd)
                        {
                            arrayStart.Pop();
                            if (arrayStart.Count <= 0)
                            {
                                _jsonReader.Read();
                                break;
                            }

                        }
                        if (_jsonReader.Token == JsonToken.ArrayStart)
                        {
                            arrayStart.Push(JsonToken.ArrayStart);
                        }
                    }

                    break;
                case JsonToken.String:
                case JsonToken.Double:
                case JsonToken.Int:
                case JsonToken.Long:
                    _jsonReader.Read();
                    break;
                default:
                    _jsonReader.Read();
                    break;
            }
        }

        private void DeserializeCompositeMap(object root, FieldDescriptor currentFieldDescriptor)
        {
            String arrayTag = _jsonReader.Value != null ? _jsonReader.Value.ToString() : null;
            _jsonReader.Read();
            Object subRoot;

            if (currentFieldDescriptor.IsPolymorphic)
            {
                if (!currentFieldDescriptor.IsWrapped)
                {
                    _jsonReader.Read();
                }

                while (_jsonReader.Token != JsonToken.ArrayEnd)
                {
                    _jsonReader.Read();
                    String tagName = _jsonReader.Value.ToString();

                    _jsonReader.Read();

                    subRoot = GetSubRoot(currentFieldDescriptor, tagName);

                    if (subRoot is IMappable)
                    {
                        Object key = ((IMappable) subRoot).Key();
                        IDictionary collection =
                            (IDictionary) currentFieldDescriptor.AutomaticLazyGetCollectionOrMap(root);
                        collection.Add(key, subRoot);
                    }

                    _jsonReader.Read();
                    _jsonReader.Read();
                }
            }
            else
            {
                while (_jsonReader.Read() && _jsonReader.Token != JsonToken.ArrayEnd)
                {
                    subRoot = GetSubRoot(currentFieldDescriptor, arrayTag);
                    if (subRoot is IMappable)
                    {
                        Object key = ((IMappable) subRoot).Key();
                        IDictionary collection =
                            (IDictionary) currentFieldDescriptor.AutomaticLazyGetCollectionOrMap(root);
                        collection.Add(key, subRoot);
                    }

                    if (currentFieldDescriptor.IsWrapped)
                        _jsonReader.Read();
                }
            }
        }

        private void DeserializeCompositeCollection(object root, FieldDescriptor currentFieldDescriptor)
        {
            String arrayTag = _jsonReader.Value != null ? _jsonReader.Value.ToString() : null;

            _jsonReader.Read();
            Object subRoot;

            if (currentFieldDescriptor.IsPolymorphic)
            {
                if (!currentFieldDescriptor.IsWrapped)
                {
                    _jsonReader.Read();
                }

                while (_jsonReader.Token != JsonToken.ArrayEnd)
                {
                    _jsonReader.Read();
                    String tagName = _jsonReader.Value.ToString();

                    _jsonReader.Read();

                    subRoot = GetSubRoot(currentFieldDescriptor, tagName);
                    IList collection = (IList) currentFieldDescriptor.AutomaticLazyGetCollectionOrMap(root);
                    collection.Add(subRoot);

                    _jsonReader.Read();
                    _jsonReader.Read();
                }
            }
            else
            {
                while (_jsonReader.Read() && _jsonReader.Token != JsonToken.ArrayEnd)
                {
                    subRoot = GetSubRoot(currentFieldDescriptor, arrayTag);
                    IList collection = (IList) currentFieldDescriptor
                                                   .AutomaticLazyGetCollectionOrMap(root);
                    collection.Add(subRoot);
                }
                if (currentFieldDescriptor.IsWrapped)
                    _jsonReader.Read();
            }
        }

        private void DeserializeScalarCollection(object root, FieldDescriptor currentFieldDescriptor)
        {
            _jsonReader.Read();
            while (_jsonReader.Read() && _jsonReader.Token != JsonToken.ArrayEnd)
            {
                currentFieldDescriptor.AddLeafNodeToCollection(root, _jsonReader.Value.ToString(), translationContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        private void DeserializeComposite(object root, FieldDescriptor currentFieldDescriptor)
        {
            String tagName = _jsonReader.Value.ToString();

            _jsonReader.Read();
            

            Object subRoot = GetSubRoot(currentFieldDescriptor, tagName);
            currentFieldDescriptor.SetFieldToComposite(root, subRoot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFieldDescriptor"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        private object GetSubRoot(FieldDescriptor currentFieldDescriptor, string tagName)
        {
            _jsonReader.Read();
            Object subRoot = null;

            if (_jsonReader.Token == JsonToken.PropertyName)
            {
                if(_jsonReader.Value.ToString().Equals(TranslationContext.JsonSimplRef))
                {
                    _jsonReader.Read();
                    subRoot = translationContext.GetFromMap(_jsonReader.Value.ToString());
                    _jsonReader.Read();
                }
                else
                {
                    ClassDescriptor subRootClassDescriptor = currentFieldDescriptor.ChildClassDescriptor(tagName);
                    subRoot = subRootClassDescriptor.GetInstance();
                    CreateObjectModel(subRoot, subRootClassDescriptor);    
                }
            }

            return subRoot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        private void DeserializeScalar(object root, FieldDescriptor currentFieldDescriptor)
        {
            _jsonReader.Read();
            currentFieldDescriptor.SetFieldToScalar(root, _jsonReader.Value.ToString(), translationContext);
        }
    }
}
