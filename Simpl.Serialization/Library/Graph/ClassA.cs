using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph
{
    public class ClassA
    {
        [SimplScalar] private int x;

        [SimplScalar] private int y;

        [SimplComposite] private ClassB classB;

        [SimplComposite] private ClassA classA;

        public ClassA()
        {

        }

        public ClassA(int pX, int pY)
        {
            x = pX;
            y = pY;
            classA = this;
        }

        public ClassA(int pX, int pY, ClassB pClassB)
        {
            x = pX;
            y = pY;
            classB = pClassB;
            classA = this;
        }

        public ClassB ClassB
        {
            set { classB = value; }
        }
    }
}
