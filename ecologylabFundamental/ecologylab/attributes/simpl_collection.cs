using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.attributes
{
    /// <summary>
    ///     Annotation describes a class field as a collection. 
    ///     This includes Lists which can be both mono-morphic and 
    ///     poly-morphic
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class simpl_collection : Attribute
    {
        private String tagName;

        /// <summary>
        /// 
        /// </summary>
        public simpl_collection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        public simpl_collection(String tagName)
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
