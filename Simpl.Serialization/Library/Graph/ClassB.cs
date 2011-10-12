using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph
{
    public class ClassB
    {
        [SimplScalar] private int a;

        [SimplScalar] private int b;

        [SimplComposite] private ClassA classA;

        public ClassB()
        {

        }

        public ClassB(int pA, int pB)
        {
            a = pA;
            b = pB;
        }

        public ClassB(int pA, int pB, ClassA pClassA)
        {
            a = pA;
            b = pB;
            classA = pClassA;
        }

        public ClassA ClassA
        {
            set { classA = value; }
        }
    }
}
