namespace ecologylab.serialization.sax
{
    /// <summary>
    ///     Interface to handle errors raised by SAX Parser
    /// </summary>
    public interface ISAXErrorHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void warning(SAXParseException exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void error(SAXParseException exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void fatalError(SAXParseException exception);
    }
}
