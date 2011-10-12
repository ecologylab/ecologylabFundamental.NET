using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph.Diamond
{
    public class ClassA
    {
        [SimplScalar] private int x;

        [SimplScalar] private int y;

        [SimplComposite] private ClassC classC;

        public ClassA()
        {

        }

        public ClassA(ClassC pClassC)
        {
            x = 1;
            y = 2;
            classC = pClassC;
        }
    }
}
