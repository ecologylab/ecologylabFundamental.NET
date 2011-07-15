using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.attributes
{
    /// <summary>
    /// Annotation which describes which equals operator is to be used to comapre objects 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class simpl_use_equals_equals : Attribute
    {

    }
}
