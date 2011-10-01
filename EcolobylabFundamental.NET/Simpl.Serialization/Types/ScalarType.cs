using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Types
{
    public abstract class ScalarType : SimplType
    {
        [SimplScalar] private Boolean _isPrimitive;

        /// <summary>
        ///     Default value for reference type objects is considered to be null
        /// </summary>
        public const Object DefaultValue = null;

        /// <summary>
        ///     When translating null objects the serialized value is String null
        /// </summary>
        public const String DefaultValueString = "null";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpType"></param>
        /// <param name="javaTypeName"></param>
        /// <param name="objectiveCTypeName"></param>
        /// <param name="dbTypeName"></param>
        protected ScalarType(Type cSharpType, String javaTypeName, String objectiveCTypeName, String dbTypeName)
            : base(cSharpType, true, javaTypeName, objectiveCTypeName, dbTypeName)
        {
            _isPrimitive = cSharpType.IsPrimitive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="unmarshallingContext"></param>
        /// <returns></returns>
        public abstract Object GetInstance(String value, String[] formatStrings,
                                           IScalarUnmarshallingContext unmarshallingContext);

        public Object GetInstance(String value)
        {
            return GetInstance(value, null, null);
        }

        /// <summary>
        ///     Sets the value of the field in the context 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="field"></param>
        /// <param name="valueString"></param>
        /// <param name="format"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns></returns>
        public virtual Boolean SetField(Object context, String valueString, String[] format,
                                        IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return SetField(context, default(FieldInfo), valueString, format, scalarUnmarshallingContext);
        }

        /// <summary>
        ///     Sets the value of the field in the context 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="field"></param>
        /// <param name="valueString"></param>
        /// <param name="format"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns></returns>
        public virtual Boolean SetField(Object context, FieldInfo field, String valueString, String[] format,
                                        IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            if (valueString == null)
                return true;

            Boolean result = false;

            try
            {
                Object referenceObject = GetInstance(valueString, format, scalarUnmarshallingContext);
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
        ///     Outputs the erorr message for the raised exception. 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="e"></param>
        protected void SetFieldError(FieldInfo field, String value, Exception e)
        {
            Console.WriteLine("Got " + e + " while trying to set field " + field + " to " + value);
        }

        /// <summary>
        ///     True if the abstracted type is a primitive type
        /// </summary>
        public Boolean IsPrimitive
        {
            get { return _isPrimitive; }
        }

        /// <summary>
        ///     True if the abstracted type is a reference type
        /// </summary>
        public Boolean IsReference
        {
            get { return !_isPrimitive; }
        }

        /// <summary>
        ///     Serializes the objects to its string value 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual String Marshall(object instance)
        {
            return instance.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string DeriveJavaTypeName()
        {
            return JavaTypeName ?? CSharpTypeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string DeriveObjectiveCTypeName()
        {
            return ObjectiveCTypeName ?? SimplName;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool IsScalar
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="fieldDescriptor"></param>
        /// <param name="context"></param>
        /// <param name="format"></param>
        public void AppendValue(StringBuilder buffy, FieldDescriptor fieldDescriptor, ElementState context,
                                Format format)
        {
            Object instance = fieldDescriptor.Field.GetValue(context);
            AppendValue(instance, buffy, !fieldDescriptor.IsCDATA, format);
        }

        /// <summary>
        ///     Appends the marshalled value of the object to output buffer
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="buffy"></param>
        /// <param name="needsEscaping"></param>
        /// <param name="format">Format serializing to, each one has their own escaping requirements.</param>
        public void AppendValue(object instance, StringBuilder buffy, Boolean needsEscaping, Format format)
        {
            String marshalled = Marshall(instance);
            if (!needsEscaping)
                buffy.Append(marshalled);
            else
            {

                String result = null;
                switch (format)
                {
                    case Format.Xml:
                        result = SecurityElement.Escape(marshalled);
                        break;
                    case Format.Json:
                        result = marshalled.Replace("\"", "\\\"");
                        break;
                }
                buffy.Append(result);
            }
        }
    }
}
