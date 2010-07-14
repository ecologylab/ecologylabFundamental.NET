using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.serialization;

namespace ecologylabFundamental.ecologylab.serialization.types.scalar
{
    /// <summary>
    /// 
    /// </summary>
    abstract class ReferenceType : ScalarType
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        public ReferenceType(Type thatClass) : base(thatClass)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="buffy"></param>
        /// <param name="needsEscaping"></param>
        public new void AppendValue(object instance, StringBuilder buffy, Boolean needsEscaping)
        {
            String instaceString = Marshall(instance);
            if (NeedsEscaping)
            {
                XMLTools.EscapeXML(buffy, instaceString);
            }
            else
            {
                buffy.Append(instaceString);
            }
        } 
    }
}
