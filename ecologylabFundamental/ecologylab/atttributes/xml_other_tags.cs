using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class xml_other_tags : Attribute
    {
        private String[] otherTags;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherTags"></param>
        public xml_other_tags(String[] otherTags)
        {
            this.otherTags = otherTags;
        }

        /// <summary>
        /// 
        /// </summary>
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
