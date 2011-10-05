namespace ecologylab.serialization.sax
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISAXContentHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="locator"></param>
        void setDocumentLocator(Locator locator);

        /// <summary>
        /// 
        /// </summary>
        void startDocument();

        /// <summary>
        /// 
        /// </summary>
        void endDocument();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>
        void processingInstruction(string target, string data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="uri"></param>
        void startPrefixMapping(string prefix, string uri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        void endPrefixMapping(string prefix);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespaceURI"></param>
        /// <param name="localName"></param>
        /// <param name="rawName"></param>
        /// <param name="atts"></param>
        void startElement(string namespaceURI, string localName,
            string rawName, Attributes atts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespaceURI"></param>
        /// <param name="localName"></param>
        /// <param name="rawName"></param>
        void endElement(string namespaceURI, string localName, string rawName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void characters(char[] ch, int start, int end);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void ignorableWhitespace(char[] ch, int start, int end);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        void skippedEntity(string name);
    }
}