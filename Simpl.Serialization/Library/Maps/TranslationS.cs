using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Maps
{
    public class TranslationS
    {
        [SimplNoWrap] 
        [SimplMap("class_descriptor")]
        public Dictionary<String, ClassDes> entriesByTag;

        public TranslationS()
        {
            entriesByTag = new Dictionary<String, ClassDes>();
        }
    }
}
