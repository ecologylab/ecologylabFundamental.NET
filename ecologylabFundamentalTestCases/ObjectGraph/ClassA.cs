using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylab.attributes;

namespace ecologylabFundamentalTestCases.ObjectGraph
{
    [simpl_inherit]
    public class ClassA : ElementState
    {
        [simpl_scalar]
        private int x;

        [simpl_scalar] 
        private int y;
    
        [simpl_composite]
        private ClassA classA;
    
        [simpl_composite]
        private ClassB classB;

        public ClassA(int x, int y)
        {
            this.x = x;
            this.y = y;            
        }

        public ClassA ClassAProp
        {
            get { return classA; }
            set { classA = value; }
        }

        public ClassB ClassBProp
        {
            get { return classB; }
            set { classB = value; }
        }
    }
}
