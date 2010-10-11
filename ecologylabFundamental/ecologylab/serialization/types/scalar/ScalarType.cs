using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ecologylabFundamental.ecologylab.serialization.types
{
    /// <summary>
    ///     Abstract class encapsulating all the scalar type fields 
    ///     in C#. String is also considered as scalar type because it 
    ///     beharves as value type objects. 
    /// </summary>
    public abstract class ScalarType
    {
        protected Type thatClass;
        protected Boolean isPrimitive;

        /// <summary>
        ///     Default value for reference type objects is considered to be null
        /// </summary>
        public static Object DEFAULT_VALUE = null;

        /// <summary>
        ///     When translating null objects the serialized value is String null
        /// </summary>
        public static String DEFAULT_VALUE_STRING = "null";

        /// <summary>
        ///     Constructor initialzes protected members. Takes input the 
        ///     class this scalar tyep object represents. 
        /// </summary>
        /// <param name="thatClass"></param>
        protected ScalarType(Type thatClass)
        {
            this.thatClass = thatClass;           
            this.isPrimitive = thatClass.IsPrimitive;
        }

        /// <summary>
        ///     Abstract method to create the instance of the class type this object
        ///     represents. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <returns></returns>
        abstract public Object GetInstance(String value, String[] formatStrings);

        /// <summary>
        ///     Abstract method to create the instance of the class type this object
        ///     represents. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Object GetInstance(String value)
        {
            return GetInstance(value, null);
        }

        /// <summary>
        ///     Sets the value of the field in the context 
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
        ///     Sets the value of the field in the context 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns>Boolean</returns>
        public Boolean SetField(Object obj, FieldInfo field, String value)
        {
            return SetField(obj, field, value, null);
        }

        /// <summary>
        ///     Outputs the erorr message for the raised exception. 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="e"></param>
        protected void SetFieldError(FieldInfo field, String value, Exception e)
        {
            System.Console.WriteLine("Got " + e + " while trying to set field " + field + " to " + value);
        }

        /// <summary>
        ///     True if the abstracted type is a primitive type
        /// </summary>
        public Boolean IsPrimitive
        {
            get
            {
                return isPrimitive;
            }
        }

        /// <summary>
        ///     True if the abstracted type is a reference type
        /// </summary>
        public Boolean IsReference
        {
            get
            {
                return !isPrimitive;
            }
        }

        /// <summary>
        ///     True if the abstracted type needs escaping. 
        /// </summary>
        public Boolean NeedsEscaping
        {
            get
            {
                return IsReference;
            }
        }

        /// <summary>
        ///     Gets the type encapsulated by this class. 
        /// </summary>
        public Type EncapsulatedType
        {
            get
            {
                return thatClass;
            }
        }

        /// <summary>
        ///     Checks if the value of the field in context is equal to its default value. 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsDefaultValue(FieldInfo field, ElementState context)
        {
            return field.GetValue(context) == null;
        }

        /// <summary>
        ///     Appends the value of the field represented by the field descriptor to
        ///     the output buffer. 
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
        ///     Serializes the objects to its string value 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected String Marshall(object instance)
        {
            return instance.ToString();
        }

        /// <summary>
        ///     Appends the marshalled value of the object to output buffer
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="buffy"></param>
        /// <param name="needsEscaping"></param>
        public void AppendValue(object instance, StringBuilder buffy, Boolean needsEscaping)
        {
            buffy.Append(Marshall(instance));
        }

        /// <summary>
        /// TODO: if not required then remove.
        /// </summary>
        public bool IsMarshallOnly { get; set; }
    }
}