using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml.library
{
    /// <summary>
    /// 
    /// </summary>
    public class RssState : ElementState
    {
        [serial_attribute]
        private float version;

        [serial_nested]
        private Channel channel;

        /// <summary>
        /// 
        /// </summary>
        public RssState()
        {
          
        }

        /// <summary>
        /// 
        /// </summary>
        public float Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Channel Channel
        {
            get
            {
                return channel;
            }
            set
            {
                channel = value;
            }
        }
    }
}
