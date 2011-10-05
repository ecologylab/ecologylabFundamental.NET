using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.serialization.json
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

    /// <summary>
    /// 
    /// </summary>
    public class JSONParseException
    {
        /// <summary>
        /// 
        /// </summary>
        private String message;

        /// <summary>
        /// 
        /// </summary>
        public JSONParseException()
        {
            message = "";
        }

        /// <summary>
        /// 
        /// </summary>
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
