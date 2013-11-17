using System;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Circle
{
    public class Point
    {
        [SimplScalar]
        [SimplHints(new[] { Hint.XmlLeaf })]
        [SimplTag("x")]
        private readonly double _x;

        [SimplScalar]
        [SimplHints(new[] { Hint.XmlLeaf })]
        [SimplTag("y")]
        private readonly double _y;

        public Point()
        {
            
        }

        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }
    }
}
