using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.serialization.json
{
    /// <summary>
    ///     Interface to handle errors raised by SAX Parser
    /// </summary>
    public interface IJSONErrorHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void warning(JSONParseException exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void error(JSONParseException exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void fatalError(JSONParseException exception);
    }

    public class JSONParseException
    {
        private String message;

        public JSONParseException()
        {
            message = "";
        }

        public String Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }
    }

}
