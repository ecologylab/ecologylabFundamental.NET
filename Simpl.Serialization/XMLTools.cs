using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using Simpl.Serialization.Attributes;
using ecologylab.serialization;

namespace Simpl.Serialization
{
    /// <summary>
    ///     Class with static utility methods. These methods encapsulate some repeated 
    ///     tasks during marshalling and unmarshalling processes. 
    /// </summary>
    public class XmlTools
    {

        static readonly String[] EscapeTable = new String[Char.MaxValue];

        static XmlTools()
        {
            for (int c = 0; c < ISO_LATIN1_START; c++)
            {
                switch (c)
                {
                    case '\"':
                        EscapeTable[c] = "&quot;";
                        break;
                    case '\'':
                        EscapeTable[c] = "&#39;";
                        break;
                    case '&':
                        EscapeTable[c] = "&amp;";
                        break;
                    case '<':
                        EscapeTable[c] = "&lt;";
                        break;
                    case '>':
                        EscapeTable[c] = "&gt;";
                        break;
                    case '\n':
                        EscapeTable[c] = "&#10;";
                        break;
                    case TAB:
                    case CR:
                        break;
                }
            }
        }

        public static String EscapeJson(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                switch (ch)
                {
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '/':
                        sb.Append("\\/");
                        break;
                    default:
                        if ((ch >= '\u0000' && ch <= '\u001F') || (ch >= '\u007F' && ch <= '\u009F') ||
                            (ch >= '\u2000' && ch <= '\u20FF'))
                        {
                            String ss = IntToHexString(ch);
                            sb.Append("\\u");
                            for (int k = 0; k < 4 - ss.Length; k++)
                            {
                                sb.Append('0');
                            }
                            sb.Append(ss.ToUpper());
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        private static String IntToHexString(int n)
        {
            int num;
            char[] hex = new char[4];

            for (int i = 0; i < 4; i++)
            {
                num = n % 16;

                if (num < 10)
                    hex[3 - i] = (char)('0' + num);
                else
                    hex[3 - i] = (char)('A' + (num - 10));

                n >>= 4;
            }

            return new string(hex);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="thatReferenceObject"></param>
        /// <returns></returns>
        public static ICollection GetCollection(Object thatReferenceObject)
        {
            ICollection result;

            if (thatReferenceObject is IList)
            {
                result = (IList)thatReferenceObject;
            }
            else
            {
                result = ((IDictionary)thatReferenceObject).Values;
            }
            return result;
        }

        /// <summary>
        ///     Returns true if the given type of annotation is present
        ///     on the specified class
        /// </summary>
        /// <param name="thatClass"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Boolean IsAnnotationPresent(Type thatClass, Type attributeType)
        {
            Object[] attributes = thatClass.GetCustomAttributes(attributeType, true);
            if (attributes != null && attributes.Length > 0) return true;
            else return false;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="thatField"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Boolean IsAnnotationPresent(FieldInfo thatField, Type attributeType)
        {
            Object[] attributes = thatField.GetCustomAttributes(attributeType, true);
            if (attributes.Length > 0) return true;
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatField"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Attribute GetAnnotation(FieldInfo thatField, Type attributeType)
        {
            Object[] attributes = thatField.GetCustomAttributes(attributeType, true);
            if (attributes.Length > 0)
            {
                return (Attribute)attributes[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        /// <param name="attributeType"></param>
        /// <param name="considerInheritedAnnotations"></param>
        /// <returns></returns>
        public static Attribute GetAnnotation(Type thatClass, Type attributeType, bool considerInheritedAnnotations = false)
        {
            Object[] attributes = thatClass.GetCustomAttributes(attributeType, considerInheritedAnnotations);
            if (attributes != null && attributes.Length > 0)
            {
                return (Attribute)attributes[0];
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisClass"></param>
        /// <returns></returns>
        public static String[] OtherTags(Type thisClass)
        {
            SimplOtherTags otherTagsAnnotation = null;
            object[] attributes = thisClass.GetCustomAttributes(false);

            foreach (Attribute attribute in attributes)
            {
                if (attribute is SimplOtherTags)
                {
                    otherTagsAnnotation = (SimplOtherTags)attribute;
                    break;
                }
            }

            return otherTagsAnnotation == null ? null : OtherTags(otherTagsAnnotation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherTagsAttributes"></param>
        /// <returns></returns>
        public static String[] OtherTags(SimplOtherTags otherTagsAttributes)
        {
            String[] result = otherTagsAttributes == null ? null : otherTagsAttributes.OtherTags;
            if ((result != null) && (result.Length == 0))
                result = null;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string GetXmlTagName(Type thatClass, String suffix)
        {
            SimplTag tagAnnotation = (SimplTag)GetAnnotation(thatClass, typeof(SimplTag));
            String result = null;

            if (tagAnnotation != null)
                result = tagAnnotation.TagName;

            if (result == null)
            {
                result = GetXmlTagName(GetClassName(thatClass), suffix);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private static int DEFAULT_TAG_LENGTH = 15;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string GetXmlTagName(string className, string suffix)
        {
            if ((suffix != null) && (className.EndsWith(suffix)))
            {
                int suffixPosition = className.LastIndexOf(suffix);
                className = className.Substring(0, suffixPosition);
            }

            StringBuilder result = new StringBuilder(DEFAULT_TAG_LENGTH);

            int classNameLength = className.Length;
            for (int i = 0; i < classNameLength; i++)
            {
                char c = className.ToCharArray()[i];

                if ((c >= 'A') && (c <= 'Z'))
                {
                    char lc = Char.ToLower(c);
                    if (i > 0)
                        result.Append('_');
                    result.Append(lc);
                }
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        static Dictionary<String, String> classAbbrevNames = new Dictionary<String, String>();

        /// <summary>
        /// 
        /// </summary>
        static Dictionary<String, String> packageNames = new Dictionary<String, String>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        /// <returns></returns>
        private static String GetClassName(Type thatClass)
        {
            String fullName = thatClass.Name;
            String abbrevName = null;
            if (!classAbbrevNames.TryGetValue(fullName, out abbrevName))
            {
                abbrevName = thatClass.Name;

                classAbbrevNames.Add(fullName, abbrevName);
            }
            return abbrevName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetXmlTagName(FieldInfo field)
        {
            SimplTag tagAnnotation = (SimplTag)GetAnnotation(field, typeof(SimplTag));

            String result = null;
             
            if (tagAnnotation != null)
                result = tagAnnotation.TagName;

            if (result == null)
            {
                result = GetXmlTagName(field.Name, null);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string[] GetFormatAnnotation(FieldInfo field)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="describedClass"></param>
        /// <returns></returns>
        public static Object GetInstance(Type describedClass)
        {
            return Activator.CreateInstance(describedClass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatField"></param>
        /// <returns></returns>
        public static bool IsScalar(FieldInfo thatField)
        {
            return IsAnnotationPresent(thatField, typeof(SimplScalar));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool IsEnum(FieldInfo field)
        {
            return IsEnum(field.FieldType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static bool IsEnum(Type fieldType)
        {
            return fieldType.IsEnum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Hint SimplHint(FieldInfo field)
        {
            SimplHints hintAnnotation = (SimplHints)GetAnnotation(field, typeof(SimplHints));
            return (hintAnnotation == null) ? Hint.XmlAttribute : hintAnnotation.Value[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="instaceString"></param>
        public static void EscapeXML(StringBuilder buffy, String instaceString)
        {

        }

        public static bool IsCompositeAsScalarValue(FieldInfo field)
	    {
		    return IsAnnotationPresent(field, typeof(SimplCompositeAsScalar));
	    }

        const int ISO_LATIN1_START = 128;
        const char TAB = (char)0x09;
        const char LF = (char)0x0a;
        const char CR = (char)0x0d;

        static Boolean NoCharsNeedEscaping(char[] stringToEscape)
        {
            int length = stringToEscape.Length;
            for (int i = 0; i < length; i++)
            {
                char c = stringToEscape[i];
                if (c >= ISO_LATIN1_START)
                {
                    return false;
                }
                else if (EscapeTable[c] != null)
                {
                    return false;
                }
                else
                {
                    switch (c)
                    {
                        case TAB:
                        case CR:
                            break;
                        default:
                            if (c < 0x20)
                                return false;
                            break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public static Enum CreateEnumeratedType(FieldInfo field, string valueString)
        {
            if (IsEnum(field))
            {
                Array enumArray = field.FieldType.GetEnumValues();

                foreach (Enum enumObj in enumArray)
                {
                    if (enumObj.ToString().Equals(valueString))
                    {
                        return enumObj;
                    }
                }
            }
            return null;
        }

        public static String CamelCaseFromXMLElementName(String elementName, bool capsOn)
        {
            StringBuilder result = new StringBuilder(DEFAULT_TAG_LENGTH);

            for (int i = 0; i < elementName.Length; i++)
            {
                char c = elementName[i];

                if (capsOn)
                {
                    result.Append(Char.ToUpper(c));
                    capsOn = false;
                }
                else
                {
                    if (c != '_')
                        result.Append(c);
                }
                if (c == '_')
                    capsOn = true;
            }
            return result.ToString();
        }

        public static String FieldNameFromElementName(String elementName)
        {
            return CamelCaseFromXMLElementName(elementName, false);
        }
    }
}
