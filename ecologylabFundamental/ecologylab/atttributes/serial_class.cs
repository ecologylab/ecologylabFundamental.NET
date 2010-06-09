using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    /// <summary>
    /// Annotation to bind the class with type of the field. 
    /// Used for generic types
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class serial_class : Attribute
    {
         private Type m_class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_class"></param>
        public serial_class(Type p_class)
        {
            this.m_class = p_class;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type Class
        {
            get
            {
                return m_class;
            }
        }
    }
}
