using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylab.attributes;

namespace ecologylabFundamentalTestCases.ObjectGraph
{
    [simpl_inherit]
    public class ClassB : ElementState
    {
        [simpl_scalar] 
        private int a;

        [simpl_scalar]
        private int b;

        [simpl_composite]
        private ClassA classA;

        public ClassB(int x, int y, ClassA classA)
        {
            this.a = x;
            this.b = y;
            this.classA = classA;
        }
    }
}
