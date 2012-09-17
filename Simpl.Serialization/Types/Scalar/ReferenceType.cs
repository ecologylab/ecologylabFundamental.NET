using System;
using System.Text;

namespace Simpl.Serialization.Types.Scalar
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
        /// <param name="javaTypeName"></param>
        /// <param name="objectiveCTypeName"></param>
        /// <param name="dbTypeName"></param>
        protected ReferenceType(Type thatClass, String javaTypeName, String objectiveCTypeName, String dbTypeName)
            : base(thatClass, javaTypeName, objectiveCTypeName, dbTypeName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="buffy"></param>
        /// <param name="needsEscaping"></param>
        public void AppendValue(object instance, StringBuilder buffy, Boolean needsEscaping)
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
