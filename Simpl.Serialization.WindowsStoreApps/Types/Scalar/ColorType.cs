using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Simpl.Serialization.Context;
using Windows.UI;
using System.Reflection;

namespace Simpl.Serialization.Types.Scalar
{
    public class ColorType : ColorTypeBase
    {
        public ColorType() : this(typeof(Color), CLTypeConstants.JavaColor, CLTypeConstants.ObjCColor, null)
        {
        }

        public ColorType(Type thatClass, string javaTypeName, string objectiveCTypeName, string dbTypeName) : base(thatClass, javaTypeName, objectiveCTypeName, dbTypeName)
        {
        }

        public override object GetInstance(string value, string[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            string hex = value.Replace("#", "");

            bool hasAlpha = (hex.Length >= 8);
            try
            {
                var intValue = Convert.ToUInt32(hex, 16);
                if (!hasAlpha)
                    intValue += 0xff000000;

                return Color.FromArgb(Convert.ToByte((intValue & 0xff000000) >> 24), Convert.ToByte((intValue & 0x00ff0000 >> 16)), Convert.ToByte((intValue & 0x0000ff00) >> 8), Convert.ToByte(intValue & 0x000000ff));
            }
            catch (Exception)
            {
                return (value.StartsWith("rgb")) ? RgbToBrush(value) : ColorStringToBrush(value);
            }
        }

        private Color? RgbToBrush(string value)
        {
            var values = value.Split(',', '(', ')');
            return (values.Length > 3) ? Color.FromArgb(255, byte.Parse(values[1]), byte.Parse(values[2]), byte.Parse(values[3])) : Colors.Black;

        }

        public Color? ColorStringToBrush(string name)
        {
            if (name.Length > 1)
            {
                name = char.ToUpper(name[0]) + name.Substring(1);
                var property = typeof (Colors).GetRuntimeProperty(name);
                if (property != null)
                {
                    return (Color) property.GetValue(null);
                }
            }
            return null;
        }
            

        public override string Marshall(object instance, TranslationContext context = null)
        {
            Color color = instance is Color ? (Color) instance : new Color();
            return color.ToString();
        }
    }
}
