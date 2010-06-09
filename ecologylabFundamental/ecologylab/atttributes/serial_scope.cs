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
    public class serial_scope : Attribute
    {
        private String translationScope;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationScope"></param>
        public serial_scope(String translationScope)
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
