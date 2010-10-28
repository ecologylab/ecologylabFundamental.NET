using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylabFundamental.ecologylab.serialization.types.scalar
{
    class EnumeratedType : ReferenceType
    {

        public EnumeratedType()
            : base(typeof(Enum))
        {

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="field"></param>
        /// <param name="valueString"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public override Boolean SetField(Object context, FieldInfo field, String valueString, String[] format)
        {
            Boolean returnValue = false;

            if (valueString == null)
                returnValue = true;
            else
            {
                Enum referenceObject;

                try
                {
                    referenceObject = XMLTools.CreateEnumeratedType(field, valueString);
                    if (referenceObject != null)
                    {
                        field.SetValue(context, referenceObject);
                        returnValue = true;
                    }
                }
                catch (System.Exception e)
                {
                    SetFieldError(field, valueString, e);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns></returns>
        public override object GetInstance(string value, string[] formatStrings)
        {
            return null;
        }
    }
}
