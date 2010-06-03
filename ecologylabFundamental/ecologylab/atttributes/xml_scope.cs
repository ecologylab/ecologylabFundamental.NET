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
    public class xml_scope : Attribute
    {
        private String translationScope;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationScope"></param>
        public xml_scope(String translationScope)
        {
            this.translationScope = translationScope;
        }

        /// <summary>
        /// 
        /// </summary>
        public String TranslationScope
        {
            get
            {
                return translationScope;
            }
        }
    }
}
