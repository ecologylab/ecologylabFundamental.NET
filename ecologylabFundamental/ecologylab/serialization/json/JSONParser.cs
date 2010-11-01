using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using System.IO;

namespace ecologylab.serialization.json
{
    class JSONParser
    {
        private IJSONContentHandler handler;
        private IJSONErrorHandler   errorHandler;
        private List<Boolean> inArray;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void parse(String url)
        {
            FileStream fileStream = File.OpenRead(url);
            parse(fileStream);
        }

        public void parse(FileStream fileStream)
        {
            parse(new StreamReader(fileStream));
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
       
        private void push(Boolean var)
        {
            inArray.Add(var);
        }

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
