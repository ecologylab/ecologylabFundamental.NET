namespace ecologylabFundamental.ecologylab.xml.sax
{
    public interface ISAXErrorHandler
    {
        void warning(SAXParseException exception);

        void error(SAXParseException exception);

        void fatalError(SAXParseException exception);
    }
}
