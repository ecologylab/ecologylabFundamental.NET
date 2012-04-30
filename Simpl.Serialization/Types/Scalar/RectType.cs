using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using Simpl.Fundamental.Net;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    public class RectType : ReferenceType
    {
        public RectType()
            : base(typeof(Rect), CLTypeConstants.JavaRectangle, CLTypeConstants.ObjCRect, null)
        {
        }

        public override Object GetInstance(String value, String[] formatStrings, IScalarUnmarshallingContext scalarUnmarshallingContext)
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
    }
}
