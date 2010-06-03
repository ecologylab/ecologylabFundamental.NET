using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class xml_scope : Attribute
    {
        private String translationScope;

        public xml_scope(String translationScope)
        {
            this.translationScope = translationScope;
        }

        public String TranslationScope
        {
            get
            {
                return translationScope;
            }
        }
    }
}
