using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph.Diamond
{
    public class ClassX
    {
        [SimplScalar] private int w;

        [SimplScalar] private int u;


        public ClassX()
        {
            w = 44;
            u = 33;
        }
    }
}
