using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class xml_map : Attribute
    {
        private String tagName;

        public xml_map(String tagName)
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
