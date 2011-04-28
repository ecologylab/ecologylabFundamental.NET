using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class simpl_composite_as_scalar : Attribute
    {
       
    }
}
