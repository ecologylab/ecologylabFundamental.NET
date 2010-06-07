using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.atttributes
{
    /// <summary>
    /// Annotation which describes a class whose super classes also
    /// contains the annotation meta-language which needs to be resolved. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class xml_inherit : Attribute
    {

    }
}
