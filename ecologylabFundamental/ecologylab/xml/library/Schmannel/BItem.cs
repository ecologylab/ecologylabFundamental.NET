using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml.library.Schmannel
{
    /// <summary>
    /// 
    /// </summary>
    
    [serial_inherit]
    public class BItem : Item
    {
        /// <summary>
        /// 
        /// </summary>
        public BItem()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        public BItem(String title)
            : base(title)
        {

        }
    }
}
