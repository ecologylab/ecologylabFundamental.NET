using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.attributes;

namespace ecologylabFundamental.ecologylab.serialization.library
{
    /// <summary>
    /// 
    /// </summary>
    public class RssState : ElementState
    {
        [simpl_scalar]
        [simpl_hints(new Hint[] { Hint.XML_ATTRIBUTE })]
        private float version;

        [simpl_composite]
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
