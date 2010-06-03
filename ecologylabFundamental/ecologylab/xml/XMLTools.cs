using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.xml
{
    class XMLTools
    {
        public static void EscapeXML(StringBuilder output, StringBuilder textNode)
        {
            //throw new NotImplementedException();
        }

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

        public static Boolean IsAnnotationPresent(Type thatClass, Type attributeType)
        {
            Object[] attributes = thatClass.GetCustomAttributes(attributeType, true);
            if (attributes != null && attributes.Length > 0) return true;
            else return false;
        }

        public static Boolean IsAnnotationPresent(FieldInfo thatField, Type attributeType)
        {
            Object[] attributes = thatField.GetCustomAttributes(attributeType, true);
            if (attributes != null && attributes.Length > 0) return true;
            else return false;
        }

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

        public static String[] OtherTags(xml_other_tags otherTagsAttributes)
        {
            String[] result = otherTagsAttributes == null ? null : otherTagsAttributes.OtherTags;
            if ((result != null) && (result.Length == 0))
                result = null;
            return result;
        }

        internal static string GetXmlTagName(Type thatClass, String suffix)
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


        private static int DEFAULT_TAG_LENGTH 		= 15;

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

        static Dictionary<String, String> classAbbrevNames = new Dictionary<String, String>();
        static Dictionary<String, String> packageNames = new Dictionary<String, String>();

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


        internal static string GetXmlTagName(FieldInfo field)
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

        internal static string[] GetFormatAnnotation(FieldInfo field)
        {
            return null;
        }

        internal static bool LeafIsCDATA(FieldInfo field)
        {
            xml_leaf leafAnnotation = (xml_leaf) GetAnnotation(field, typeof(xml_tag));
            return ((leafAnnotation != null) && (leafAnnotation.Value == ElementState.CDATA));

        }

        internal static ElementState GetInstance(Type describedClass)
        {
            return (ElementState) Activator.CreateInstance(describedClass);
        }
    }
}
