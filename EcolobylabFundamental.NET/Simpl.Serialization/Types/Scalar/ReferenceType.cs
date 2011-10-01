using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization;
using Simpl.Serialization.Types;
using ecologylab.serialization;

namespace ecologylab.serialization.types.scalar
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ReferenceType : ScalarType
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
            String instanceString = Marshall(instance);
            if (NeedsEscaping)
            {
                XmlTools.EscapeXML(buffy, instanceString);
            }
            else
            {
                buffy.Append(instanceString);
            }
        } 
    }
}
