using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using LitJson;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Deserializers.PullHandlers.StringFormats
{
    public class JsonPullDeserializer : StringPullDeserializer
    {
        private JsonReader _jsonReader;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        public JsonPullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext,
                                    IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputTranslationScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTranslationScope"></param>
        /// <param name="inputContext"></param>
        public JsonPullDeserializer(TranslationScope inputTranslationScope, TranslationContext inputContext)
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
            ClassDescriptor rootClassDescriptor = translationScope.GetClassDescriptorByTag(_jsonReader.Value.ToString());

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
            Object subRoot = null;
            while (_jsonReader.Token != JsonToken.ObjectEnd)
            {
                FieldDescriptor currentFieldDescriptor = rootClassDescriptor.GetFieldDescriptorByTag(_jsonReader.Value.ToString());
                switch (currentFieldDescriptor.FdType)
                {
                    case FieldTypes.Scalar:
                        DeserializeScalar(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CompositeElement:
                        DeserializeComposite(root, currentFieldDescriptor);
                        break;
                }
                _jsonReader.Read();
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

            if(_jsonReader.Token == JsonToken.PropertyName)
            {
                ClassDescriptor subRootClassDescriptor = currentFieldDescriptor.ChildClassDescriptor(tagName);
                subRoot = subRootClassDescriptor.GetInstance();
                CreateObjectModel(subRoot, subRootClassDescriptor);
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
