using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylabFundamental.ecologylab.xml.types
{
    abstract class ScalarType
    {
        Type thatClass;
        Type alternativeClass;

        Boolean isPrimitive;

        public static Object DEFAULT_VALUE = null;
        public static String DEFAULT_VALUE_STRING = "null";

        protected ScalarType(Type thatClass)
        {
            this.thatClass = thatClass;           
            this.isPrimitive = thatClass.IsPrimitive;
        }

        abstract public Object GetInstance(String value, String[] formatStrings);

        public Object GetInstance(String value)
        {
            return GetInstance(value, null);
        }

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

        public Boolean SetField(Object obj, FieldInfo field, String value)
        {
            return SetField(obj, field, value, null);
        }

        protected void SetFieldError(FieldInfo field, String value, Exception e)
        {
            System.Console.WriteLine("Got " + e + " while trying to set field " + field + " to " + value);
        }

        //Properties
        public Boolean IsPrimitive
        {
            get
            {
                return isPrimitive;
            }
        }

        public Boolean IsReference
        {
            get
            {
                return !isPrimitive;
            }
        }

        public Boolean NeedsEscaping
        {
            get
            {
                return IsReference;
            }
        }

        public Type EncapsulatedType
        {
            get
            {
                return thatClass;
            }
        }

        public bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return field.GetValue(context) == null;
        }

        public void AppendValue(StringBuilder buffy, FieldDescriptor fieldDescriptor, ElementState context)
        {
            Object instance = fieldDescriptor.Field.GetValue(context);
            AppendValue(instance, buffy, !fieldDescriptor.IsCDATA);
        }        

        private String Marshall(object instance)
        {
            return instance.ToString();
        }

        internal void AppendValue(object instance, StringBuilder buffy, Boolean needsEscaping)
        {
            buffy.Append(Marshall(instance));
        }

        public bool IsMarshallOnly { get; set; }
    }
}
