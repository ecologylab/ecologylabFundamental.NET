using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Distributed.Common
{
    static class NetworkConstants
    {
        /// <summary>
        /// the maximum size of message acceptable by server in encoded CHARs
        /// </summary>
        public const int DefaultMaxMessageLengthChars = 128*1024;    //128KB

        public static readonly int DefaultIdleTimeout = 10000;

        /// <summary>
        /// The maximum size an http-like header on a message may be, in bytes.
        /// </summary>
        public static readonly int MaxHttpHeaderLength = 4 * 1024; //4KB

        /// <summary>
        /// The content-length http-like header indicator.
        /// </summary>
        public static readonly String ContentLengthString = "content-length";

        public static readonly String UniqueIdentifierString = "uid";

        public static readonly String HttpHeaderLineDelimiter = "\r\n";

        /// <summary>
        /// The terminator string for the end of http-like headers.
        /// </summary>
        public static readonly String HttpheaderTerminator = HttpHeaderLineDelimiter + HttpHeaderLineDelimiter;

        /// <summary>
        /// Content coding specifies whether or not to some type of comression is used in the message
        /// </summary>
        public static readonly String HttpContentCoding = "content-encoding";
        
        /// <summary>
        /// Specifies what decoding schemes are acceptable to send back to the the client
        /// </summary>
        public static readonly String HttpAcceptedEncodings = "accept-encoding:deflate";

        public static readonly String HttpAcceptEncoding = "accept_encoding";

        /// <summary>
        /// String specifying deflate encoding
        /// </summary>
        public static readonly String HttpDeflateEncoding = "deflate";

        /// <summary>
        /// The size of the content-length header indicator.
        /// </summary>
        public static readonly int ContentLengthStringLength = ContentLengthString.Length;

        /// <summary>
        /// Character encoding for messages sent through the network.
        /// </summary>
        public static readonly String CharacterEncoding = "ISO-8859-1";

        public static readonly Encoding Charset = Encoding.GetEncoding(CharacterEncoding);
    }
}
