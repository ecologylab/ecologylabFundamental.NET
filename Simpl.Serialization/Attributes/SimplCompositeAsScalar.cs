using System;

namespace Simpl.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimplCompositeAsScalar : Attribute
    {
       
    }
}
