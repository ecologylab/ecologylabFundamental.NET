using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.xml;

namespace ecologylabFundamental.ecologylab.atttributes
{
    /// <summary>
    /// Defines a field is represented as XML leaf when marshalled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class serial_leaf : Attribute
    {
        private int m_value;

        /// <summary>
        /// 
        /// </summary>
        public serial_leaf()
        {
            this.m_value = ElementState.NORMAL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_value"></param>
        public serial_leaf(int p_value)
        {
            this.m_value = p_value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Value
        {
            get
            {
                return m_value;
            }
        }
    }
}
