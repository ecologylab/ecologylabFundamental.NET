namespace ecologylabFundamental.ecologylab.xml.sax
{
    public interface ISAXContentHandler
    {
        void setDocumentLocator(Locator locator);

        void startDocument();

        void endDocument();

        void processingInstruction(string target, string data);

        void startPrefixMapping(string prefix, string uri);

        void endPrefixMapping(string prefix);

        void startElement(string namespaceURI, string localName,
            string rawName, Attributes atts);

        void endElement(string namespaceURI, string localName, string rawName);

        void characters(char[] ch, int start, int end);

        void ignorableWhitespace(char[] ch, int start, int end);

        void skippedEntity(string name);
    }
}