using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    /// <summary>
    ///     Defines a new tag on a field. By default the field name is taken as 
    ///     the attribute of the field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class xml_tag : Attribute
    {
        private String tagName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        public xml_tag(String tagName)
        {
            this.tagName = tagName;
        }

        /// <summary>
        /// 
        /// </summary>
        public String TagName
        {
            get
            {
                return tagName;
            } 
        }
    }
}