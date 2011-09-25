using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.attributes
{
    /// <summary>
    ///     Takes a translation scope which binds classes to their XML
    ///     representation. This is used for polymorphic types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class simpl_scope : Attribute
    {
        private String translationScope;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationScope"></param>
        public simpl_scope(String translationScope)
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
