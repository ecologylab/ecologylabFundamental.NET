using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Serialization.Context;
using System.Windows;

namespace Simpl.Serialization.Types.Scalar
{
    class RectType : RectTypeBase
    {
        public RectType()
            : base(typeof(Rect), CLTypeConstants.JavaRectangle, CLTypeConstants.ObjCRect, null)
        {
        }

        public override object GetInstance(string value, string[] formatStrings, IScalarUnmarshallingContext unmarshallingContext)
        {
            Object result = null;
            try
            {
                double x = 0, y = 0, width = 0, height = 0;
                string[] values = value.Split(' ');

                if (values.Length > 1)
                {
                    x = double.Parse(values[0]);
                    y = double.Parse(values[1]);
                }

                if (values.Length > 3)
                {
                    width = double.Parse(values[2]);
                    height = double.Parse(values[3]);
                }

                result = new Rect(x, y, width, height);
            }
            catch (ArgumentNullException e) { }
            catch (ArgumentException e) { }

            return result;
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((Rect)instance).ToString();
        }
    }
}
