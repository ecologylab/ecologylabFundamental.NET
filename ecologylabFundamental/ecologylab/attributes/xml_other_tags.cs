using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.attributes
{
    /// <summary>
    ///     Annotation which defines other tags in serialized form
    ///     This is used to support old XML files.
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
