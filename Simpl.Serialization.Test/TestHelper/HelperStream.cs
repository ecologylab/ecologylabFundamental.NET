using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Simpl.Serialzation.Tests.TestHelper
{
    /// <summary>
    /// a simple helper stream class to manage data between strings and binary formats, for printing outputs to console. 
    /// </summary>
    public class HelperStream : MemoryStream
    {
        /// <summary>
        /// 
        /// </summary>
        public HelperStream()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public HelperStream(byte[] b) : base(b)
        {
            
        }

        /// <summary>
        /// return the data as string using default system encoding
        /// </summary>
        public String StringData
        {
            get { return Encoding.Default.GetString(base.ToArray()); }
        }

        /// <summary>
        /// return the data of the stream as a byte array. 
        /// </summary>
        public byte[] BinaryData
        {
            get { return base.ToArray(); }
        }
    }
}
