using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library
{
    public class Point
    {
        [SimplScalar] private int x;
        [SimplScalar] private int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
