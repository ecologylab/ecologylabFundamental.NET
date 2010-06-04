namespace ecologylabFundamental.ecologylab.xml.sax
{
    /// <summary>
    ///     Interface to handle errors raised by SAX Parser
    /// </summary>
    public interface ISAXErrorHandler
    {
        void warning(SAXParseException exception);

        void error(SAXParseException exception);

        void fatalError(SAXParseException exception);
    }
}
