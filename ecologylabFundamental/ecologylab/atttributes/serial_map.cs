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
    public class serial_map : Attribute
    {
        private String tagName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        public serial_map(String tagName)
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
