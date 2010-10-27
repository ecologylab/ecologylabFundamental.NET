using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.attributes;

namespace ecologylabFundamental.ecologylab.serialization.library.Schmannel
{
    /// <summary>
    /// 
    /// </summary>
    
    [simpl_inherit]
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
