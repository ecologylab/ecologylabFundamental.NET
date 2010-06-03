using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class xml_classes : Attribute
    {
        private Type[] classes;

        public xml_classes(Type[] classes)
        {
            this.classes = classes;
        }

        public Type[] Classes
        {
            get
            {
                return classes;
            }
        }
    }
}
