using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Serialization.Context;
using Windows.UI;

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
            var intValue = Convert.ToUInt32(hex, 16);
            if (!hasAlpha)
                intValue += 0xff000000;
            
            //var a = hasAlpha ? Convert.ToByte(hex.Substring(0, 2), 16) : Byte.MaxValue;
            //int i = hasAlpha ? 2 : 0;
            //var r = Convert.ToByte(hex.Substring(i, 2), 16);
            //i += 2;
            //var g = Convert.ToByte(hex.Substring(i, 2), 16);
            //i += 2;
            //var b = Convert.ToByte(hex.Substring(i, 2), 16);

            return Color.FromArgb(Convert.ToByte((intValue & 0xff000000) >> 24), Convert.ToByte((intValue & 0x00ff0000 >> 16)), Convert.ToByte((intValue & 0x0000ff00) >> 8), Convert.ToByte(intValue & 0x000000ff));
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            Color color = instance is Color ? (Color) instance : new Color();
            return "#" + color.ToString();
        }
    }
}
