using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    class xml_other_tags : Attribute
    {
        private String[] otherTags;

        public xml_other_tags(String[] otherTags)
        {
            this.otherTags = otherTags;
        }

        public String[] OtherTags
        {
            get
            {
                return otherTags;
            }
            set
            {
                otherTags = value;
            }
        }
    }
}
