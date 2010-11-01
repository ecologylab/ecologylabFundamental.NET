using System;
using System.Xml;
using System.Collections;
using ecologylab.serialization.sax;
using System.Web;
using System.IO;

namespace ecologylab.serialization.sax
{
    /// <summary>
    ///    The SaxParser class build a SAX push model from the pull model found
    ///    in the XmlTextReader.
    /// </summary>
    public class SaxParser
    {
        private ISAXContentHandler Handler = null;
        private ISAXErrorHandler errorHandler = null;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorHandler"></param>
        public void setErrorHandler(ISAXErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public void setContentHandler(ISAXContentHandler handler)
        {
            this.Handler = handler;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        public void parse(Stream inputStream)
        {
            int buflen = 500;
            char[] buffer = new char[buflen];
            Stack nsstack = new Stack();
            Locator locator = new Locator();
            SAXParseException saxException = new SAXParseException();
            Attributes atts;
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(inputStream);
                object nsuri = reader.NameTable.Add("http://www.w3.org/2000/xmlns/");
                Handler.startDocument();
                while (reader.Read())
                {
                    string prefix;
                    locator.LineNumber = reader.LineNumber;
                    locator.ColumnNumber = reader.LinePosition;
                    Handler.setDocumentLocator(locator);
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            nsstack.Push(null);//marker
                            atts = new Attributes();
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.NamespaceURI.Equals(nsuri))
                                {
                                    prefix = "";
                                    if (reader.Prefix == "xmlns")
                                    {
                                        prefix = reader.LocalName;
                                    }

                                    nsstack.Push(prefix);
                                    Handler.startPrefixMapping(prefix, reader.Value);
                                }
                                else
                                {
                                    SaxAttribute newAtt = new SaxAttribute();
                                    newAtt.Name = reader.Name;
                                    newAtt.NamespaceURI = reader.NamespaceURI;
                                    newAtt.Value = reader.Value;
                                    atts.attArray.Add(newAtt);
                                }
                            }
                            reader.MoveToElement();
                            Handler.startElement(reader.NamespaceURI,
                                reader.LocalName, reader.Name, atts.TrimArray());
                            if (reader.IsEmptyElement)
                            {
                                Handler.endElement(reader.NamespaceURI,
                                    reader.LocalName, reader.Name);
                            }
                            break;
                        case XmlNodeType.EndElement:
                            Handler.endElement(reader.NamespaceURI,
                                reader.LocalName, reader.Name);
                            prefix = (string)nsstack.Pop();
                            while (prefix != null)
                            {
                                Handler.endPrefixMapping(prefix);
                                prefix = (string)nsstack.Pop();
                            }
                            break;
                        case XmlNodeType.Text:
                            char[] characters = reader.ReadString().ToCharArray();
                            Handler.characters(characters, 0, characters.Length);
                            //After read your are automatically put on the next tag so you
                            //have to call the proper case from here or it won't work correctly.
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                goto case XmlNodeType.Element;
                            }
                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                goto case XmlNodeType.EndElement;
                            }
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            Handler.processingInstruction(reader.Name, reader.Value);
                            break;
                        case XmlNodeType.Whitespace:
                            char[] whiteSpace = reader.Value.ToCharArray();
                            Handler.ignorableWhitespace(whiteSpace, 0, 1);
                            break;
                        case XmlNodeType.Entity:
                            Handler.skippedEntity(reader.Name);
                            break;
                    }
                }
                Handler.endDocument();
            } //try
            catch (Exception exception)
            {
                saxException.LineNumber = reader.LineNumber.ToString();
                saxException.SystemID = "";
                saxException.Message = exception.GetBaseException().ToString();
                errorHandler.error(saxException);
            }
            finally
            {
                if (reader.ReadState != ReadState.Closed)
                {
                    reader.Close();
                }
            }
        }
    }
}
