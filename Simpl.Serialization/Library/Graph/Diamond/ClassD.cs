using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph.Diamond
{
    public class ClassD
    {
        [SimplComposite] private ClassA classA;

        [SimplComposite] private ClassB classB;


        public ClassD()
        {

        }

        public ClassD(ClassA pClassA, ClassB pClassB)
        {
            classA = pClassA;
            classB = pClassB;
        }
    }
}
