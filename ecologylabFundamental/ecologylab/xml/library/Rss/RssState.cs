using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml.library
{
    public class RssState : ElementState
    {
        [xml_attribute]
        private float version;

        [xml_nested]
        private Channel channel;

        public RssState()
        {
          
        }

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
