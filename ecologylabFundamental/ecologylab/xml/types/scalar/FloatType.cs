using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml.types.scalar
{
    /// <summary>
    /// 
    /// </summary>
    class FloatType : ScalarType
    {
        /// <summary>
        /// 
        /// </summary>
        public FloatType()
            : base(typeof(float))
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns></returns>
        public override Object GetInstance(String value, String[] formatStrings)
        {
            return float.Parse(value);       
        }
    }
}
