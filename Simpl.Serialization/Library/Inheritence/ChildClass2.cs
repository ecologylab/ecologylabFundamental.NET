using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Inheritence
{
    [SimplInherit]
    public class ChildClass2 : BaseClass
    {
        [SimplScalar]
        [SimplHints(new Hint[] { Hint.XmlLeaf })]
        private int ccvar2 = 1;

        public ChildClass2()
        {
        }
    }
}
