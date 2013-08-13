using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    public class DateTimeType : ReferenceType
    {
        public static readonly DateTime UtcEpoch            = new DateTime(1970, 1, 1);
        public new static readonly DateTime DefaultValue    = DateTime.Now;
        public new const String DefaultValueString          = "0";

        /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public DateTimeType()
            : this(typeof (DateTime))
        {
        }

        public DateTimeType(Type type)
            : base(type, CLTypeConstants.JavaDate, CLTypeConstants.ObjCDate, null)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns></returns>
        public override object GetInstance(string value, string[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            try
            {
                DateTime result;
                try
                {
                   result = Convert.ToDateTime(value);
                }
                catch (FormatException e)
                {
                    result = UtcEpoch.AddSeconds(Convert.ToInt64(value));
                }
               
                return result;
            }
            catch(FormatException e)
            {
                return null;
            }
            
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            var unixTime = Convert.ToInt64((((DateTime) instance).ToUniversalTime() - UtcEpoch).TotalSeconds);
            return unixTime.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, Object context)
        {
            return (DateTime) field.GetValue(context) == DefaultValue;
        }


        public override bool SimplEquals(object left, object right)
        {
            return base.GenericSimplEquals<DateTime>(left, right);
        }

        public override bool NeedsJsonQuotationWrap()
        {
            return false;
        }

    }
}
