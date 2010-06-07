using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    /// <summary>
    /// Annotation describes a class field as a collection. 
    /// This includes Lists which can be both mono-morphic and 
    /// poly-morphic
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class xml_collection : Attribute
    {
        private String tagName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        public xml_collection(String tagName)
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
