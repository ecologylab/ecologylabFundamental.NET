using System;
using System.Collections.Generic;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.Types.Element;

namespace Simpl.Serialization.Library.Maps
{
    public class ClassDes : IMappable
    {
        [SimplScalar] 
        public String tagName;

        [SimplNoWrap] 
        [SimplMap("field_descriptor")]
        public DictionaryList<String, FieldDes> fieldDescriptorsByTagName;

        public ClassDes()
        {
            tagName = "";
            fieldDescriptorsByTagName = new DictionaryList<String, FieldDes>();
        }

        public ClassDes(String tagName)
        {
            this.tagName = tagName;
            fieldDescriptorsByTagName = new DictionaryList<String, FieldDes>();
        }


        public Object Key()
        {
            return tagName;
        }
    }
}
