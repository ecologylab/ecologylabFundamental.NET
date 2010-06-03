using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml
{
    /// <summary>
    /// 
    /// </summary>
    public class XMLTools
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="textNode"></param>
        public static void EscapeXML(StringBuilder output, StringBuilder textNode)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatReferenceObject"></param>
        /// <returns></returns>
        public static ICollection GetCollection(Object thatReferenceObject)
        {
            ICollection result = null;

            if(thatReferenceObject is IList)
            {
                result = (IList) thatReferenceObject;
            }
            else
            {
                result = ((IDictionary)thatReferenceObject).Values;
            }
            return result;
        }

        /// <summary>
        /// 
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
            xml_tag tagAnnotation = (xml_tag) GetAnnotation(thatClass, typeof(xml_tag));
            String result = null;

            if(tagAnnotation != null)
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
        private static int DEFAULT_TAG_LENGTH 		= 15;

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
            xml_tag tagAnnotation = (xml_tag) GetAnnotation(field, typeof(xml_tag));

            String result = null;

            if(tagAnnotation != null)
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
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool LeafIsCDATA(FieldInfo field)
        {
            xml_leaf leafAnnotation = (xml_leaf) GetAnnotation(field, typeof(xml_tag));
            return ((leafAnnotation != null) && (leafAnnotation.Value == ElementState.CDATA));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="describedClass"></param>
        /// <returns></returns>
        public static ElementState GetInstance(Type describedClass)
        {
            return (ElementState) Activator.CreateInstance(describedClass);
        }
    }
}
