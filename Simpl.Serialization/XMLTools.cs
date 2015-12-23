using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Attributes;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="stringToEscape"></param>
        public static void EscapeXml(StringBuilder buffy, String stringToEscape)
        {
            if (NoCharsNeedEscaping(stringToEscape.ToCharArray()))
            {
                buffy.Append(stringToEscape);
            }
            else
            {
                int length = stringToEscape.Length;
                for (int i = 0; i< length; ++i)
                {
                    char c = stringToEscape[i];
                    string escaped = EscapeTable[c];

                    if (escaped != null)
                        buffy.Append(escaped);
                    else
                    {
                        switch (c)
                        {
                            case TAB:
                            case CR:
                                buffy.Append(c);
                                break;
                            default:
                                if (c >= 255)
                                {
                                    int cInt = (int)c;
                                    buffy.Append('&').Append('#').Append(cInt).Append(';');
                                }
                                else if (c >= 0x20)
                                    buffy.Append(c);
                                break;
                        }
                    }
                }
            }
        }

        public static string EscapeXml(String s)
        {
            StringBuilder sb = new StringBuilder();
            EscapeXml(sb, s);
            return sb.ToString();
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
                    //case '/':
                    //    sb.Append("\\/");
                    //    break;
                    default:
                        if ((ch >= '\u0000' && ch <= '\u001F') || (ch >= '\u007F' && ch <= '\u009F') ||
                            (ch >= '\u2000' && ch <= '\u20FF'))
                        {
                            String ss = ((Int32) ch).HexString();
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
            if (thatReferenceObject == null)
                return null;

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




        public static Boolean IsAnnotationPresent<SimplAnnotation>(Type thatClass) where SimplAnnotation : Attribute
        {
            return IsAnnotationPresent(thatClass, typeof(SimplAnnotation));
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
            IEnumerable<Attribute> attributes = thatClass.GetTypeInfo().GetCustomAttributes(attributeType, true);
            return attributes.Any();
        }



        public static Boolean IsAnnotationPresent<SimplAnnotation>(FieldInfo thatField) where SimplAnnotation : Attribute
        {
            return IsAnnotationPresent(thatField, typeof(SimplAnnotation));
        }
        
        public static Boolean IsAnnotationPresent(FieldInfo thatField, Type attributeType)
        {
            IEnumerable<Attribute> attributes = thatField.GetCustomAttributes(attributeType, true);
            return attributes.Any();
        }



        public static SimplAnnotation GetAnnotation<SimplAnnotation>(FieldInfo thatField) where SimplAnnotation : Attribute
        {
            return (SimplAnnotation)GetAnnotation(thatField, typeof(SimplAnnotation));
        }

        public static Attribute GetAnnotation(FieldInfo thatField, Type attributeType)
        {
            IEnumerable<Attribute> attributes = thatField.GetCustomAttributes(attributeType, true);
            var enumerable = attributes as List<Attribute> ?? attributes.ToList();
            if (attributes != null && enumerable.Any())
            {
                return enumerable.First();
            }
            else
            {
                return null;
            }
        }


        public static SimplAnnotation GetAnnotation<SimplAnnotation>(Type thatClass, bool considerInheritedAnnotations = false) where SimplAnnotation : Attribute
        {
            return (SimplAnnotation)GetAnnotation(thatClass, typeof(SimplAnnotation), considerInheritedAnnotations);
        }
        
        public static Attribute GetAnnotation(Type thatClass, Type attributeType, bool considerInheritedAnnotations = false)
        {
            IEnumerable<Attribute> attributes = thatClass.GetTypeInfo().GetCustomAttributes(attributeType, considerInheritedAnnotations);
            if (attributes != null && attributes.Any())
            {
                return attributes.First();
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
            IEnumerable<Attribute> attributes = thisClass.GetTypeInfo().GetCustomAttributes(false);

            SimplOtherTags otherTagsAnnotation = attributes.OfType<SimplOtherTags>().FirstOrDefault();

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
            SimplTag tagAnnotation = GetAnnotation<SimplTag>(thatClass);
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
            SimplTag tagAnnotation = GetAnnotation<SimplTag>(field);

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
            try
            {
                Object result = Activator.CreateInstance(describedClass);
                return result;
            }
            catch(Exception e)
            {
                Debug.WriteLine("Simpl Error. Failed to create intance of " + describedClass.Name);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatField"></param>
        /// <returns></returns>
        public static bool IsScalar(FieldInfo thatField)
        {
            return IsAnnotationPresent<SimplScalar>(thatField);
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
            return fieldType.GetTypeInfo().IsEnum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Hint SimplHint(FieldInfo field)
        {
            SimplHints hintAnnotation = GetAnnotation<SimplHints>(field);
            return (hintAnnotation == null) ? Hint.XmlAttribute : hintAnnotation.Value[0];
        }



        public static bool IsCompositeAsScalarValue(FieldInfo field)
	    {
		    return IsAnnotationPresent<SimplCompositeAsScalar>(field);
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
                var enumArray = field.FieldType.GetEnumValues();

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

        public static int CalculateHash(String input)
        {
            int h = 0;
            int len = input.Length;
            for (int i = 0; i < len; i++)
            {
                h = 31 * h + input[i];
            }
            return h;
        }
    }
}
