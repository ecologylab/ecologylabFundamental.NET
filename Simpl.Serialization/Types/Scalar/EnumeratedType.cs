using System;
using System.Reflection;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    class EnumeratedType : ReferenceType
    {

        public EnumeratedType()
            : base(typeof(Enum), null, null, null)
        {
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="context"></param>
       /// <param name="field"></param>
       /// <param name="valueString"></param>
       /// <param name="format"></param>
       /// <param name="scalarUnmarshallingContext"></param>
       /// <returns></returns>
        public override Boolean SetField(Object context, FieldInfo field, String valueString, String[] format, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            Boolean returnValue = false;

            if (valueString == null)
                returnValue = true;
            else
            {
                Enum referenceObject;

                try
                {
                    referenceObject = XmlTools.CreateEnumeratedType(field, valueString);
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

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return instance.ToString();
        }

        /// <summary>
/// 
/// </summary>
/// <param name="value"></param>
/// <param name="formatStrings"></param>
/// <param name="scalarUnmarshallingContext"></param>
/// <returns></returns>
        public override object GetInstance(String value, string[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return null;
        }
    }
}
