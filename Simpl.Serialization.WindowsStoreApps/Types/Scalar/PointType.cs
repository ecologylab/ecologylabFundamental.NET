using System;
using Simpl.Serialization.Context;
using Windows.Foundation;

namespace Simpl.Serialization.Types.Scalar
{
    public class PointType : PointTypeBase
    {
        public PointType()
            : base(typeof(Point), CLTypeConstants.JavaRectangle, CLTypeConstants.ObjCRect, null)
        {
        }

        public override object GetInstance(string value, string[] formatStrings, IScalarUnmarshallingContext unmarshallingContext)
        {
            Object result = null;
            try
            {
                double x = 0, y = 0;
                string[] values = value.Split(',');

                if (values.Length > 1)
                {
                    x = double.Parse(values[0]);
                    y = double.Parse(values[1]);
                }

                result = new Point(x, y);
            }
            catch (ArgumentNullException e) { }
            catch (ArgumentException e) { }

            return result;
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((Point)instance).ToString();
        }
    }
}
