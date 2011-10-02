using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library
{
    public class Circle
    {
        [SimplComposite]
        private Point p;

        [SimplScalar]
        [SimplHints(new Hint[] {Hint.XmlLeaf})]
        private int r;

        public Circle(Point p, int r)
        {
            this.p = p;
            this.r = r;
        }
    }
}
