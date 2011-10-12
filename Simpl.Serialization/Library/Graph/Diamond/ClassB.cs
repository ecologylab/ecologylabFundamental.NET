using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph.Diamond
{
    public class ClassB
    {
        [SimplScalar] private int x;

        [SimplScalar] private int y;

        [SimplComposite] private ClassC classC;

        [SimplComposite] private ClassX classX;

        public ClassB()
        {

        }

        public ClassB(ClassC pClassC)
        {
            x = 1;
            y = 2;
            classC = pClassC;
            classX = new ClassX();
        }
    }
}
