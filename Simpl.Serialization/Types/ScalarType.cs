using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types
{
    public abstract class ScalarType : SimplType
    {
        /// <summary>
        /// Determines if a simpl Collection Type can be created for the given C# TYpe
        /// </summary>
        /// <param name="aType">The type to consider</param>
        /// <returns>True if a collection type can be made</returns>
        public static bool CanBeCreatedFrom(Type aType)
        {
            return !CollectionType.CanBeCreatedFrom(aType);
        }

        [SimplScalar] private Boolean _isPrimitive;

        /// <summary>
        ///     Default value for reference type objects is considered to be null
        /// </summary>
        public static readonly Object DefaultValue = null;

        /// <summary>
        ///     When translating null objects the serialized value is String null
        /// </summary>
        public static readonly String DefaultValueString = "null";

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
            _isPrimitive = cSharpType.GetTypeInfo().IsPrimitive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="unmarshallingContext"></param>
        /// <returns></returns>
        public abstract object GetInstance(string value, string[] formatStrings, IScalarUnmarshallingContext unmarshallingContext);

        public object GetInstance(String value)
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
            Debug.WriteLine("Got " + e + " while trying to set field " + field + " to " + value);
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
        ///     True if the abstracted type is a reference type
        /// </summary>
        public Boolean IsMarshallOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Serializes the objects to its string value 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract string Marshall(object instance, TranslationContext context = null);

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
        public bool NeedsEscaping
        {
            get { return IsReference; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldDescriptor"></param>
        /// <param name="context"></param>
        /// <param name="format"></param>
        public void AppendValue(TextWriter textWriter, FieldDescriptor fieldDescriptor, Object context,
                                Format format)
        {
            Object instance = fieldDescriptor.Field.GetValue(context);
            AppendValue(instance, textWriter, !fieldDescriptor.IsCdata, format);
        }

        /// <summary>
        ///     Appends the marshalled value of the object to output buffer
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="buffy"></param>
        /// <param name="needsEscaping"></param>
        /// <param name="format">Format serializing to, each one has their own escaping requirements.</param>
        public void AppendValue(object instance, TextWriter textWriter, Boolean needsEscaping, Format format)
        {
            String marshalled = Marshall(instance, null);
            if (!needsEscaping)
                textWriter.Write(marshalled);
            else
            {
                String result = null;
                switch (format)
                {
                    case Format.Xml:
                        result = XmlTools.EscapeXml(marshalled);
                        break;
                    case Format.Json:
                        result = XmlTools.EscapeJson(marshalled);
                        break;
                    default:
                        result = marshalled;
                        break;
                }
                textWriter.Write(result);
            }
        }

        public virtual Boolean IsDefaultValue(String value)
        {
            return (DefaultValueString.Length == value.Length) && DefaultValueString.Equals(value);
        }

        public virtual Boolean IsDefaultValue(FieldInfo field, Object context)
        {
            Object fieldValue = field.GetValue(context);
            return fieldValue == null || DefaultValueString.Equals(fieldValue.ToString());
        }

        /**
	     * The string representation for a Field of this type. Reference scalar types should NOT override
	     * this. They should simply override marshall(instance), which this method calls.
	     * <p/>
	     * Primitive types cannot create such an instance, from the value of a field, and so must
	     * override.
	     */
	    public string ToString(FieldInfo field, Object context)
	    {
		    string result = "COULDNT CONVERT!";
		    try
		    {
			    Object instance = field.GetValue(context);
			    if (instance == null)
				    result = DefaultValueString;
			    else
				    result = Marshall(instance);
		    }
		    catch (Exception e)
		    {
		        Debug.WriteLine("Error: " + e.Message);
		    }
		    return result;
	    }

        /// <summary>
        /// Utility method to compare two objects of a given T... if left and right are T, uses T's equal method. 
        /// </summary>
        /// <typeparam name="T">The type to expect</typeparam>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>Returns true if they are both equal and non-null, false otherwise.</returns>
        protected bool GenericSimplEquals<T>(object left, object right)
        {
            if (left is T && right is T)
            {
                return ((T)left).Equals((T)right);
            }
            else
            {
                return false;
            }
        }

        public virtual ScalarType OperativeScalarType
        {
            get { return this; }
        }
    }

}
