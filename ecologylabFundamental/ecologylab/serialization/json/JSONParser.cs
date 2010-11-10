using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using System.IO;

namespace ecologylab.serialization.json
{
    /// <summary>
    /// 
    /// </summary>
    class JSONParser
    {
        /// <summary>
        /// 
        /// </summary>
        private IJSONContentHandler handler;

        /// <summary>
        /// 
        /// </summary>
        private IJSONErrorHandler   errorHandler;

        /// <summary>
        /// 
        /// </summary>
        private List<Boolean> inArray;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="errorHandler"></param>
        public JSONParser(IJSONContentHandler handler, IJSONErrorHandler errorHandler)
        {
            this.handler = handler;
            this.errorHandler = errorHandler;
            this.inArray = new List<Boolean>();
        }

        public JSONParser()
        {
            this.inArray = new List<Boolean>();
        }

        public void setContentHandler(IJSONContentHandler contentHandler)
        {
            this.handler = contentHandler;
        }
        public void setErrorHandler(IJSONErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public void parse(StreamReader inputStream)
        {
            JSONParseException exception = new JSONParseException();
            try
            {
                JsonReader reader = new JsonReader(inputStream);
                handler.StartJSON();

                bool registerObjectEntryEnd = false;

                while (reader.Read())
                {
                    String type = reader.Value != null ? reader.Value.GetType().ToString() : "";

                    if (registerObjectEntryEnd && !top())
                    {
                        handler.EndObjectEntry();
                    }

                    registerObjectEntryEnd = false;

                    switch (reader.Token)
                    {
                        case JsonToken.ObjectStart:
                            push(false);
                            handler.StartObject();
                            break;
                        case JsonToken.ObjectEnd:
                            pop();
                            handler.EndObject();
                            registerObjectEntryEnd = true;
                            break;
                        case JsonToken.PropertyName:
                            handler.StartObjectEntry(reader.Value.ToString());
                            break;
                        case JsonToken.Int:
                        case JsonToken.Long:
                        case JsonToken.Double:
                        case JsonToken.Boolean:
                        case JsonToken.String:
                            handler.Primitive(reader.Value);
                            registerObjectEntryEnd = true;
                            break;
                        case JsonToken.ArrayStart:
                            handler.StartArray();
                            push(true);
                            break;
                        case JsonToken.ArrayEnd:
                            handler.EndArray();
                            pop();
                            registerObjectEntryEnd = true;
                            break;
                    }
                }
                handler.EndJSON();
            }
            catch (Exception ex)
            {
                exception.Message = ex.ToString();
                errorHandler.error(exception);
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
