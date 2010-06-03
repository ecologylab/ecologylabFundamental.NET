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
    public class xml_classes : Attribute
    {
        private Type[] classes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classes"></param>
        public xml_classes(Type[] classes)
        {
            this.classes = classes;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type[] Classes
        {
            get
            {
                return classes;
            }
        }
    }
}
