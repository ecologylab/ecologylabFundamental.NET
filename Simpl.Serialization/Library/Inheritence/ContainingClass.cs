using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Inheritence
{
    public class ContainingClass
    {
        [SimplClasses(new Type[] {typeof (BaseClass), typeof (ChildClass1), typeof (ChildClass2)})] 
        [SimplComposite] 
        private BaseClass theField;

        public ContainingClass()
        {
        }

        public BaseClass TheField
        {
            set { theField = value; }
        }
    }
}
