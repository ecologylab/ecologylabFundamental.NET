using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.xml;

namespace ecologylabFundamental.ecologylab.atttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class xml_leaf : Attribute
    {
        private int m_value;

        public xml_leaf()
        {
            this.m_value = ElementState.NORMAL;
        }


        public xml_leaf(int p_value)
        {
            this.m_value = p_value;
        }

        public int Value
        {
            get
            {
                return m_value;
            }
        }
    }
}
