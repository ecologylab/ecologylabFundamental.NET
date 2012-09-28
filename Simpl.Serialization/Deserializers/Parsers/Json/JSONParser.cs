using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using LitJson;
using System.IO;
using Newtonsoft.Json;

namespace Simpl.Serialization.Deserializers.Parsers.Json
{
    /// <summary>
    /// 
    /// </summary>
    class JsonParser
    {
        /// <summary>
        /// 
        /// </summary>
        private IJsonContentHandler handler;

        /// <summary>
        /// 
        /// </summary>
        private IJsonErrorHandler   errorHandler;

        /// <summary>
        /// 
        /// </summary>
        private List<Boolean> inArray;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="errorHandler"></param>
        public JsonParser(IJsonContentHandler handler, IJsonErrorHandler errorHandler)
        {
            this.handler = handler;
            this.errorHandler = errorHandler;
            this.inArray = new List<Boolean>();
        }

        public JsonParser()
        {
            this.inArray = new List<Boolean>();
        }

        public void setContentHandler(IJsonContentHandler contentHandler)
        {
            this.handler = contentHandler;
        }
        public void setErrorHandler(IJsonErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public void parse(StreamReader inputStream, String jsonText = null)
        {
            JsonParseException exception = new JsonParseException();
            try
            {
                JsonReader reader = null;
                if (inputStream != null)
                    reader = new JsonTextReader(inputStream);
                else if (jsonText != null)
                {
                    reader = new JsonTextReader(new StringReader(jsonText));
                }


                handler.StartJson();

                bool registerObjectEntryEnd = false;

                while (reader.Read())
                {
                    String type = reader.Value != null ? reader.Value.GetType().ToString() : "";

                    if (registerObjectEntryEnd && !top())
                    {
                        handler.EndObjectEntry();
                    }

                    registerObjectEntryEnd = false;

                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject:
                            push(false);
                            handler.StartObject();
                            break;
                        case JsonToken.EndObject:
                            pop();
                            handler.EndObject();
                            registerObjectEntryEnd = true;
                            break;
                        case JsonToken.PropertyName:
                            handler.StartObjectEntry(reader.Value.ToString());
                            break;
                        case JsonToken.Integer:
                        //case JsonToken.Long:
                        case JsonToken.Float:
                        case JsonToken.Boolean:
                        case JsonToken.String:
                            handler.Primitive(reader.Value);
                            registerObjectEntryEnd = true;
                            break;
                        case JsonToken.StartArray:
                            handler.StartArray();
                            push(true);
                            break;
                        case JsonToken.EndArray:
                            handler.EndArray();
                            pop();
                            registerObjectEntryEnd = true;
                            break;
                    }
                }
                handler.EndJson();
            }
            catch (Exception ex)
            {
                exception.Message = ex.ToString();
                errorHandler.Error(exception);
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="var"></param>
        private void push(Boolean var)
        {
            inArray.Add(var);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Boolean pop()
        {
            Boolean result = false;
            if (inArray != null && inArray.Count > 0)
            {
                result =  inArray[inArray.Count - 1];
                inArray.RemoveAt(inArray.Count - 1);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Boolean top()
        {
            Boolean result = false;
            if (inArray != null && inArray.Count > 0)
            {
                result = inArray[inArray.Count - 1];
            }

            return result;
        }
    }
}
