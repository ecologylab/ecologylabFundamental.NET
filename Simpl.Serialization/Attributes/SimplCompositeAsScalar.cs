using System;

namespace Simpl.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SimplCompositeAsScalar : Attribute
    {
       
    }
}
