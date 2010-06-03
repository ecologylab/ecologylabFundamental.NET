using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class xml_class : Attribute
    {
         private Type m_class;

        public xml_class(Type p_class)
        {
            this.m_class = p_class;
        }

        public Type Class
        {
            get
            {
                return m_class;
            }
        }
    }
}
