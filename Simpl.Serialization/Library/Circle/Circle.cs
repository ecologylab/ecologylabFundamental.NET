using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Circle
{
    public class Circle
    {
        [SimplComposite]
        [SimplTag("p")]
        private readonly Point _p;

        [SimplScalar]
        [SimplHints(new[] {Hint.XmlLeaf})]
        [SimplTag("r")]
        private readonly int _r;

        public Circle()
        {
            
        }

        public Circle(Point p, int r)
        {
            _p = p;
            _r = r;
        }

        public Point P
        {
            get { return _p; }
        }

        public int R
        {
            get { return _r; }
        }
    }
}
