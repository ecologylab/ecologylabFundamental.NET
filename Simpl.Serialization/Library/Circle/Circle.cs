using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Circle
{
    public class Circle
    {
        [SimplComposite]
        private readonly Point _p;

        [SimplScalar]
        [SimplHints(new Hint[] {Hint.XmlLeaf})]
        private readonly int _r;

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
