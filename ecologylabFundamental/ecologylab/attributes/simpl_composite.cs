using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.attributes
{
    /// <summary>
    ///     Annotation describes a field which is composed
    ///     of further fields inside which are also serializable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class simpl_composite : Attribute
    {

    }
}
