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

        private void ResolveIdsBeforeRefs()
        {
            
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

            DeserializationPreHook(root, translationContext);
		    if (deserializationHookStrategy != null)
			    deserializationHookStrategy.DeserializationPreHook(root, null);

            // move to the first field of the object.
            _jsonReader.Read();
            _jsonReader.Read();
            
            // complete the object model from root and recursively of the fields it is composed of
            DeserializeObjectFields(root, rootClassDescriptor);

            translationContext.ResolveIdsForRefObjects();

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theObject"></param>
        /// <param name="objectClassDescriptor"></param>
        private void DeserializeObjectFields(object theObject, ClassDescriptor objectClassDescriptor)
        {
            FieldDescriptor currentFieldDescriptor = null;
            bool insideWrapped = false;

            while (_jsonReader.TokenType != JsonToken.EndObject)
            {
                var currentJsonProperty = _jsonReader.Value;
                if (currentJsonProperty == null || _jsonReader.TokenType != JsonToken.PropertyName)
                    throw new SimplTranslationException("should have found PropertyName, but instead found " + _jsonReader.TokenType.ToString());
                
                var currentTag = _jsonReader.Value.ToString();
                if (!HandleSimplId(currentTag, theObject))
                {
                    currentFieldDescriptor = currentFieldDescriptor != null && currentFieldDescriptor.FdType== FieldTypes.Wrapper
                        ? currentFieldDescriptor.WrappedFd
                        : objectClassDescriptor.GetFieldDescriptorByTag(currentTag);

                    if (currentFieldDescriptor == null)
                    {
                        currentFieldDescriptor = FieldDescriptor.MakeIgnoredFieldDescriptor(currentTag);
                        Debug.WriteLine("ignoring " + currentTag);
                        _jsonReader.Skip();
                    }

                    switch (currentFieldDescriptor.FdType)
                    {
                        case FieldTypes.Scalar:
                            DeserializeScalar(theObject, currentFieldDescriptor);
                            break;
                        case FieldTypes.CompositeElement:
                            DeserializeComposite(theObject, currentFieldDescriptor);
                            if (insideWrapped)
                            {
                                insideWrapped = false;
                                _jsonReader.Read();
                            }
                            break;
                        case FieldTypes.CollectionScalar:
                            DeserializeScalarCollection(theObject, currentFieldDescriptor);
                            break;
                        case FieldTypes.CollectionElement:
                            DeserializeCompositeCollection(theObject, currentFieldDescriptor);
                            break;
                        case FieldTypes.MapElement:
                            DeserializeCompositeMap(theObject, currentFieldDescriptor);
                            break;
                        case FieldTypes.Wrapper:
                            // don't do this for collections or maps because they are always wrapped (even when nowrap is specified)
                            // and this is handled in respective method calls.
                            if (currentFieldDescriptor.WrappedFd.IsComposite)
                            {
                                _jsonReader.Read();
                                _jsonReader.Read();

                                if (_jsonReader.TokenType != JsonToken.PropertyName)
                                {
                                    // expecting property name, but found something else. not a properly wrapped composite.
                                    while(_jsonReader.TokenType != JsonToken.EndObject)
                                        _jsonReader.Skip();

                                    _jsonReader.Read();
                                    continue;
                                }

                                insideWrapped = true;
                            }

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
                // if simpl.id or non-wrapper field descriptor, advance to next token.
                if (currentFieldDescriptor == null || currentFieldDescriptor.FdType != FieldTypes.Wrapper)
                    _jsonReader.Read();
            }
            DeserializationPostHook(theObject, translationContext);
		    if (deserializationHookStrategy != null)
			    deserializationHookStrategy.DeserializationPostHook(theObject, null);
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
            if (TranslationContext.JsonSimplId.Equals(tagName))
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
            string simplId;

            if (currentFieldDescriptor.IsPolymorphic)
            {
                if (!currentFieldDescriptor.IsWrapped)
                {
                    _jsonReader.Read();
                }

                while (_jsonReader.TokenType != JsonToken.EndArray)
                {
                    _jsonReader.Read();
                    var tagName = _jsonReader.Value.ToString();

                    _jsonReader.Read();

                    subRoot = GetSubRoot(currentFieldDescriptor, tagName, out simplId);

                    AddToMapOrMarkUnresolved(root, currentFieldDescriptor, subRoot, simplId);

                    _jsonReader.Read();
                    _jsonReader.Read();
                }
            }
            else
            {
                while (_jsonReader.Read() && _jsonReader.TokenType != JsonToken.EndArray)
                {
                    subRoot = GetSubRoot(currentFieldDescriptor, arrayTag, out simplId);
                    
                    AddToMapOrMarkUnresolved(root, currentFieldDescriptor, subRoot, simplId);

                    if (currentFieldDescriptor.IsWrapped)
                        _jsonReader.Read();
                }
            }
        }

        private void AddToMapOrMarkUnresolved(object root, FieldDescriptor currentFieldDescriptor, object subRoot,
                                              string simplId)
        {
            IDictionary collection =
                (IDictionary) currentFieldDescriptor.AutomaticLazyGetCollectionOrMap(root);

            if (subRoot != null)
            {
                var mappable = subRoot as IMappable<object>;
                if (mappable != null)
                {
                    var key = mappable.Key();
                    collection.Add(key, mappable);
                }
            }
            else
                translationContext.RefObjectNeedsIdResolve(collection, null, simplId);
        }

        private bool HandleBackwardsCompatabilityForDoubleWrappedCollections()
        {
            var oldFormat = (_jsonReader.TokenType != JsonToken.StartArray);
            if (oldFormat)
            {
                _jsonReader.Read();
                _jsonReader.Read();
            }

            return oldFormat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        private void DeserializeCompositeCollection(object root, FieldDescriptor currentFieldDescriptor)
        {
            // advance to StartArray token
            _jsonReader.Read();

            var oldFormat = HandleBackwardsCompatabilityForDoubleWrappedCollections();

            int collectionCountIncludingRefs = 0;

            // advance to first StartObject token
            _jsonReader.Read();

            while (_jsonReader.TokenType != JsonToken.EndArray)
            {
                var tagName = currentFieldDescriptor.CollectionOrMapTagName;

                if (currentFieldDescriptor.IsPolymorphic)
                {
                    // advance to Property token
                    _jsonReader.Read();
                    tagName = _jsonReader.Value.ToString();

                    ClassDescriptor subRootClassDescriptor = currentFieldDescriptor.ChildClassDescriptor(tagName);
                    if (subRootClassDescriptor == null)
                    {
                        Debug.WriteLine("skipping tag: " + tagName + ". not assignable to " + currentFieldDescriptor.Name);
                        _jsonReader.Skip();
                        _jsonReader.Read();
                        continue;
                    }

                    // advance past wrap tag to StartObject token
                    _jsonReader.Read();
                }

                DeserializeAndAddToCollection(root, currentFieldDescriptor, tagName, collectionCountIncludingRefs);
                collectionCountIncludingRefs++;

                if (currentFieldDescriptor.IsPolymorphic)
                    _jsonReader.Read();

                _jsonReader.Read();

            }

            if (oldFormat)
                _jsonReader.Read();
        }

        private void DeserializeAndAddToCollection(object root, FieldDescriptor currentFieldDescriptor, string tagName, int actualCollectionSizeIncludingRefs)
        {
            string simplId;
            var collection = (IList) currentFieldDescriptor.AutomaticLazyGetCollectionOrMap(root);

            object subRoot = GetSubRoot(currentFieldDescriptor, tagName, out simplId);
            if (subRoot != null)
                collection.Add(subRoot);
            else if (simplId != null)
                translationContext.RefObjectNeedsIdResolve(collection, actualCollectionSizeIncludingRefs, simplId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        private void DeserializeScalarCollection(object root, FieldDescriptor currentFieldDescriptor)
        {
            _jsonReader.Read();

            var oldFormat = HandleBackwardsCompatabilityForDoubleWrappedCollections();

            while (_jsonReader.Read() && _jsonReader.TokenType != JsonToken.EndArray)
            {
                currentFieldDescriptor.AddLeafNodeToCollection(root, _jsonReader.Value.ToString(), translationContext);
            }

            if (oldFormat)
                _jsonReader.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        private void DeserializeComposite(object root, FieldDescriptor currentFieldDescriptor)
        {
            //while (_jsonReader.Value == null && _jsonReader.TokenType != JsonToken.EndObject)
            //_jsonReader.Read();

            String tagName = (_jsonReader.Value != null) ? _jsonReader.Value.ToString() : null;
            ClassDescriptor subRootClassDescriptor = currentFieldDescriptor.ChildClassDescriptor(tagName);
            _jsonReader.Read();
            if (subRootClassDescriptor != null)
            {
                string simplId;
                var subRoot = GetSubRoot(currentFieldDescriptor, tagName, out simplId);
                if (subRoot != null)
                    currentFieldDescriptor.SetFieldToComposite(root, subRoot);
                else if (simplId != null)
                {
                    translationContext.RefObjectNeedsIdResolve(root, currentFieldDescriptor, simplId);
                }
            }
            else
            {
                Debug.WriteLine("ignore tag: " + tagName + ". expected different tag while deserializing composite.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFieldDescriptor"></param>
        /// <param name="tagName"></param>
        /// <param name="simplId"></param>
        /// <returns></returns>
        private object GetSubRoot(FieldDescriptor currentFieldDescriptor, string tagName, out string simplId)
        {
            _jsonReader.Read();
            Object subRoot  = null;
            simplId         = null;

            if(_jsonReader.Value != null && _jsonReader.Value.ToString().Equals(TranslationContext.JsonSimplRef))
            {
                _jsonReader.Read();
                simplId = _jsonReader.Value.ToString();
                var referencedObject = translationContext.GetFromMap(simplId);
                if (referencedObject != null)
                {
                    subRoot = referencedObject;
                }
                _jsonReader.Read();
            }
            else
            {
                ClassDescriptor subRootClassDescriptor = currentFieldDescriptor.ChildClassDescriptor(tagName);
                if (subRootClassDescriptor != null)
                {
                    subRoot = subRootClassDescriptor.GetInstance();

                    DeserializationPreHook(subRoot, translationContext);
                    if (deserializationHookStrategy != null)
                        deserializationHookStrategy.DeserializationPreHook(subRoot, currentFieldDescriptor);

                    DeserializeObjectFields(subRoot, subRootClassDescriptor);    
                }
                else
                {
                    Debug.WriteLine("skipping tag: " + tagName + ". not assignable to " + currentFieldDescriptor.Name);
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