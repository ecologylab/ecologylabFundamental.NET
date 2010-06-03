using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class xml_tag : Attribute
    {
        private String tagName;

        public xml_tag(String tagName)
        {
            this.tagName = tagName;
        }

        public String TagName
        {
            get
            {
                return tagName;
            }
        }
    }
}
