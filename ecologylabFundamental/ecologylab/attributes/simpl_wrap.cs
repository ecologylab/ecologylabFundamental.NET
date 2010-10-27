using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.attributes
{
    /// <summary>
    ///     Annotation which describes the collection as wrapped 
    ///     collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class simpl_wrap : Attribute
    {

    }
}
