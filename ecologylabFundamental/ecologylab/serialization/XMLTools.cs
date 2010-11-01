using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using ecologylab.attributes;

namespace ecologylab.serialization
{
    /// <summary>
    ///     Class with static utility methods. These methods encapsulate some repeated 
    ///     tasks during marshalling and unmarshalling processes. 
    /// </summary>
    public class XMLTools
    {

        static String[] ESCAPE_TABLE = new String[Char.MaxValue];

        static XMLTools()
        {
            for (int c = 0; c < ISO_LATIN1_START; c++)
            {
                switch (c)
                {
                    case '\"':
                        ESCAPE_TABLE[c] = "&quot;";
                        break;
                    case '\'':
                        ESCAPE_TABLE[c] = "&#39;";
                        break;
                    case '&':
                        ESCAPE_TABLE[c] = "&amp;";
                        break;
                    case '<':
                        ESCAPE_TABLE[c] = "&lt;";
                        break;
                    case '>':
                        ESCAPE_TABLE[c] = "&gt;";
                        break;
                    case '\n':
                        ESCAPE_TABLE[c] = "&#10;";
                        break;
                    case TAB:
                    case CR:
                    default:
                        break;
                }
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="thatReferenceObject"></param>
        /// <returns></returns>
        public static ICollection GetCollection(Object thatReferenceObject)
        {
            ICollection result = null;

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
            if (attributes != null && attributes.Length > 0) return true;
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
        /// <param name="thatClass"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Attribute GetAnnotation(Type thatClass, Type attributeType)
        {
            Object[] attributes = thatClass.GetCustomAttributes(attributeType, false);
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
            xml_other_tags otherTagsAnnotation = null;
            object[] attributes = thisClass.GetCustomAttributes(false);

            foreach (Attribute attribute in attributes)
            {
                if (attribute is xml_other_tags)
                {
                    otherTagsAnnotation = (xml_other_tags)attribute;
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
        public static String[] OtherTags(xml_other_tags otherTagsAttributes)
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
            xml_tag tagAnnotation = (xml_tag)GetAnnotation(thatClass, typeof(xml_tag));
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
        private static string GetXmlTagName(string className, string suffix)
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
            xml_tag tagAnnotation = (xml_tag)GetAnnotation(field, typeof(xml_tag));

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
        public static ElementState GetInstance(Type describedClass)
        {
            return (ElementState)Activator.CreateInstance(describedClass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatField"></param>
        /// <returns></returns>
        public static bool IsScalar(FieldInfo thatField)
        {
            return IsAnnotationPresent(thatField, typeof(simpl_scalar));
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
            simpl_hints hintAnnotation = (simpl_hints)GetAnnotation(field, typeof(simpl_hints));
            return (hintAnnotation == null) ? Hint.XML_ATTRIBUTE : hintAnnotation.Value[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffy"></param>
        /// <param name="instaceString"></param>
        public static void EscapeXML(StringBuilder buffy, String instaceString)
        {

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
                else if (ESCAPE_TABLE[c] != null)
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

    }
}
