using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;

namespace ecologylab.attributes
{
    /// <summary>
    ///     Defines a field is represented as XML leaf when marshalled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class simpl_scalar : Attribute
    {
       
    }
}