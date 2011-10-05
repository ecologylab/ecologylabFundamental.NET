using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Rss
{
    public class Rss
    {
        [SimplScalar] private float version;

        [SimplComposite] private Channel channel;

        public Rss()
        {
        }

        public Rss(float pVersion, Channel pChannel)
        {
            version = pVersion;
            channel = pChannel;
        }
    }
}
