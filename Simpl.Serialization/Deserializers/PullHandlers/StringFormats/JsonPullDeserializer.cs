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

//            // move the first field in the document. typically it is the root element.
//            jp.nextToken();
//
//            Object root = null;
//
//            // find the classdescriptor for the root element.
//            ClassDescriptor rootClassDescriptor = translationScope.getClassDescriptorByTag(jp
//                    .getCurrentName());
//
//            root = rootClassDescriptor.getInstance();
//
//            // root.setupRoot();
//            // root.deserializationPreHook();
//            // if (deserializationHookStrategy != null)
//            // deserializationHookStrategy.deserializationPreHook(root, null);
//
//            // move to the first field of the root element.
//            jp.nextToken();
//            jp.nextToken();
//
//            // complete the object model from root and recursively of the fields it is composed of
//            createObjectModel(root, rootClassDescriptor);
//
//            return root;
            return null;
        }
    }
}
