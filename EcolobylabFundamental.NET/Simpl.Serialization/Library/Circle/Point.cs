using System;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Circle
{
    public class Point
    {
        [SimplScalar]
        [SimplHints(new[] { Hint.XmlLeaf })]
        [SimplTag("x")]
        private readonly int _x;

        [SimplScalar]
        [SimplHints(new[] { Hint.XmlLeaf })]
        [SimplTag("y")]
        private readonly int _y;

        public Point()
        {
            
        }

        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public Int64 X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }
    }
}
