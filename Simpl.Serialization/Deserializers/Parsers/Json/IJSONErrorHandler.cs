using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization.Deserializers.Parsers.Json
{
    /// <summary>
    ///     Interface to handle errors raised by SAX Parser
    /// </summary>
    public interface IJsonErrorHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void Warning(JsonParseException exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void Error(JsonParseException exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void FatalError(JsonParseException exception);
    }

    /// <summary>
    /// 
    /// </summary>
    public class JsonParseException
    {
        /// <summary>
        /// 
        /// </summary>
        public JsonParseException()
        {
            Message = "";
        }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }

}
