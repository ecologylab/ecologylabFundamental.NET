using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylabFundamental.ecologylab.xml.types
{
    /// <summary>
    /// 
    /// </summary>
    abstract class ScalarType
    {
        Type thatClass;
        Boolean isPrimitive;

        /// <summary>
        /// 
        /// </summary>
        public static Object DEFAULT_VALUE = null;

        /// <summary>
        /// 
        /// </summary>
        public static String DEFAULT_VALUE_STRING = "null";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        protected ScalarType(Type thatClass)
        {
            this.thatClass = thatClass;           
            this.isPrimitive = thatClass.IsPrimitive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns></returns>
        abstract public Object GetInstance(String value, String[] formatStrings);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Object GetInstance(String value)
        {
            return GetInstance(value, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="field"></param>
        /// <param name="valueString"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public Boolean SetField(Object context, FieldInfo field, String valueString, String[] format)
        {
            if (valueString == null)
                return true;

            Boolean result = false;
            Object referenceObject;

            try
            {
                referenceObject = GetInstance(valueString, format);
                if (referenceObject != null)
                {
                    field.SetValue(context, referenceObject);
                    result = true;
                }
            }
            catch (Exception e)
            {
                SetFieldError(field, valueString, e);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean SetField(Object obj, FieldInfo field, String value)
        {
            return SetField(obj, field, value, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="e"></param>
        protected void SetFieldError(FieldInfo field, String value, Exception e)
        {
            System.Console.WriteLine("Got " + e + " while trying to set field " + field + " to " + value);
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsPrimitive
        {
            get
            {
                return isPrimitive;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsReference
        {
            get
            {
                return !isPrimitive;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean NeedsEscaping
        {
            get
            {
                return IsReference;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type EncapsulatedType
        {
            get
            {
                return thatClass;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return field.GetValue(context) == null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="fieldDescriptor"></param>
        /// <param name="context"></param>
        public void AppendValue(StringBuilder buffy, FieldDescriptor fieldDescriptor, ElementState context)
        {
            Object instance = fieldDescriptor.Field.GetValue(context);
            AppendValue(instance, buffy, !fieldDescriptor.IsCDATA);
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private String Marshall(object instance)
        {
            return instance.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="buffy"></param>
        /// <param name="needsEscaping"></param>
        public void AppendValue(object instance, StringBuilder buffy, Boolean needsEscaping)
        {
            buffy.Append(Marshall(instance));
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMarshallOnly { get; set; }
    }
}
