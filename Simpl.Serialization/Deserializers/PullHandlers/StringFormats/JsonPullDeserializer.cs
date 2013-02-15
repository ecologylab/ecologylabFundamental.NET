using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
//using LitJson;
using Newtonsoft.Json;
using Simpl.Serialization.Context;
using Simpl.Serialization.Types.Element;

namespace Simpl.Serialization.Deserializers.PullHandlers.StringFormats
{
    public class JsonPullDeserializer : StringPullDeserializer
    {
        private JsonReader _jsonReader;

//        private String test;


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
//            test = inputString;
            ConfigureInput(new MemoryStream(Encoding.UTF8.GetBytes(inputString)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        private void ConfigureInput(Stream inputStream)
        {
            _jsonReader = new JsonTextReader(new StreamReader(inputStream));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private object Parse()
        {
            _jsonReader.Read();
            if (_jsonReader.TokenType != JsonToken.StartObject)
            {
                Debug.WriteLine("JSON Translation ERROR: not a valid JSON object. It should start with {");
            }

            // move the first field in the document. typically it is the root element.
            _jsonReader.Read();

            // find the classdescriptor for the root element.
            ClassDescriptor rootClassDescriptor = simplTypesScope.GetClassDescriptorByTag(_jsonReader.Value.ToString());

            object root = rootClassDescriptor.GetInstance();

            // move to the first field of the root element.
            _jsonReader.Read();
            _jsonReader.Read();

            DeserializationPreHook(root, translationContext);
		    if (deserializationHookStrategy != null)
			    deserializationHookStrategy.DeserializationPreHook(root, null);

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
            FieldDescriptor currentFieldDescriptor = null;
            bool insideWrapped = false;

            while (_jsonReader.TokenType != JsonToken.EndObject)
            {
                //                object jsonReaderValue = _jsonReader.Value;
                //
                //                FieldDescriptor currentFieldDescriptor =
                //                    rootClassDescriptor.GetFieldDescriptorByTag(jsonReaderValue.ToString());
                //
                //                if(currentFieldDescriptor == null)
                //                {
                //                    String ignoredTag = jsonReaderValue.ToString();
                //                    IgnoreCurrentTag();
                //                    Debug.WriteLine("WARNING: ignoring tag " + ignoredTag);
                //                    continue;
                //                }

                var currentTag = (_jsonReader.Value != null) ? _jsonReader.Value.ToString() : null;
                if (!HandleSimplId(currentTag, root))
                {
                    currentFieldDescriptor = currentFieldDescriptor != null && currentFieldDescriptor.FdType== FieldTypes.Wrapper
                        ? currentFieldDescriptor.WrappedFd
                        : rootClassDescriptor.GetFieldDescriptorByTag(currentTag);

                    if (currentFieldDescriptor == null)
                    {
                        currentFieldDescriptor = FieldDescriptor.MakeIgnoredFieldDescriptor(currentTag);
                    }

                    switch (currentFieldDescriptor.FdType)
                    {
                        case FieldTypes.Scalar:
                            DeserializeScalar(root, currentFieldDescriptor);
                            break;
                        case FieldTypes.CompositeElement:
                            DeserializeComposite(root, currentFieldDescriptor);
                            if (insideWrapped)
                            {
                                insideWrapped = false;
                                _jsonReader.Read();
                            }
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

                            insideWrapped = true;

                            //                        currentFieldDescriptor = currentFieldDescriptor.WrappedFd;
                            //                        if (currentFieldDescriptor.FdType == FieldTypes.CompositeElement)
                            //                            goto case FieldTypes.CompositeElement;
                            //                        if (currentFieldDescriptor.FdType == FieldTypes.CollectionScalar)
                            //                            goto case FieldTypes.CollectionScalar;
                            //                        if (currentFieldDescriptor.FdType == FieldTypes.CollectionElement)
                            //                            goto case FieldTypes.CollectionElement;
                            //                        if (currentFieldDescriptor.FdType == FieldTypes.MapElement)
                            //                            goto case FieldTypes.MapElement;

                            break;
                    }
                }
                _jsonReader.Read();
            }
            DeserializationPostHook(root, translationContext);
		    if (deserializationHookStrategy != null)
			    deserializationHookStrategy.DeserializationPostHook(root, null);
        }

        /**
	     * Function used for handling graph's simpl.id tag. If the tag is present the current ElementState
	     * object is marked as unmarshalled. Therefore, later simpl.ref can be used to extract this
	     * instance
	     * 
	     * @param tagName
	     * @param root
	     * @return
	     * @throws JsonParseException
	     * @throws IOException
	     */
	    private bool HandleSimplId(String tagName, Object root)
	    {
            if (TranslationContext.SimplId.Equals(tagName))
		    {
		        _jsonReader.Read();
			    translationContext.MarkAsUnmarshalled(_jsonReader.Value.ToString(), root);
			    return true;
		    }
		    return false;
	    }

        /// <summary>
        /// 
        /// </summary>
        private void IgnoreCurrentTag()
        {
            _jsonReader.Read();

            switch (_jsonReader.TokenType)
            {
                case JsonToken.StartObject:
                    Stack<JsonToken> objectStarts = new Stack<JsonToken>();
                    objectStarts.Push(JsonToken.StartObject);
                    while (_jsonReader.Read())
                    {
                        if (_jsonReader.TokenType == JsonToken.EndObject)
                        {
                            objectStarts.Pop();
                            if (objectStarts.Count <= 0)
                            {
                                _jsonReader.Read();
                                break;
                            }

                        }
                        if (_jsonReader.TokenType == JsonToken.StartObject)
                        {
                            objectStarts.Push(JsonToken.StartObject);
                        }
                    }
                    break;
                case JsonToken.StartArray:
                    Stack<JsonToken> arrayStart = new Stack<JsonToken>();
                    arrayStart.Push(JsonToken.StartArray);
                    while (_jsonReader.Read())
                    {
                        if (_jsonReader.TokenType == JsonToken.EndArray)
                        {
                            arrayStart.Pop();
                            if (arrayStart.Count <= 0)
                            {
                                _jsonReader.Read();
                                break;
                            }

                        }
                        if (_jsonReader.TokenType == JsonToken.StartArray)
                        {
                            arrayStart.Push(JsonToken.StartArray);
                        }
                    }

                    break;
                case JsonToken.String:
                case JsonToken.Float:
                case JsonToken.Integer:
                //case JsonToken.Long:
                    _jsonReader.Read();
                    break;
                default:
                    _jsonReader.Read();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
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

                while (_jsonReader.TokenType != JsonToken.EndArray)
                {
                    _jsonReader.Read();
                    String tagName = _jsonReader.Value.ToString();

                    _jsonReader.Read();

                    subRoot = GetSubRoot(currentFieldDescriptor, tagName);

                    if (subRoot is IMappable<Object>)
                    {
                        Object key = ((IMappable<Object>) subRoot).Key();
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
                while (_jsonReader.Read() && _jsonReader.TokenType != JsonToken.EndArray)
                {
                    subRoot = GetSubRoot(currentFieldDescriptor, arrayTag);
                    if (subRoot is IMappable<Object>)
                    {
                        Object key = ((IMappable<Object>) subRoot).Key();
                        IDictionary collection =
                            (IDictionary) currentFieldDescriptor.AutomaticLazyGetCollectionOrMap(root);
                        collection.Add(key, subRoot);
                    }

                    if (currentFieldDescriptor.IsWrapped)
                        _jsonReader.Read();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
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

                while (_jsonReader.TokenType != JsonToken.EndArray)
                {
                    _jsonReader.Read();
                    String tagName = _jsonReader.Value != null ? _jsonReader.Value.ToString() : null;

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
                while (_jsonReader.Read() && _jsonReader.TokenType != JsonToken.EndArray)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        private void DeserializeScalarCollection(object root, FieldDescriptor currentFieldDescriptor)
        {
            _jsonReader.Read();
            while (_jsonReader.Read() && _jsonReader.TokenType != JsonToken.EndArray)
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
            //while (_jsonReader.Value == null && _jsonReader.TokenType != JsonToken.EndObject)
            _jsonReader.Read();

            String tagName = (_jsonReader.Value != null) ? _jsonReader.Value.ToString() : null;

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

            if (_jsonReader.TokenType == JsonToken.PropertyName)
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

                    DeserializationPreHook(subRoot, translationContext);
			        if (deserializationHookStrategy != null)
				        deserializationHookStrategy.DeserializationPreHook(subRoot, currentFieldDescriptor);

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