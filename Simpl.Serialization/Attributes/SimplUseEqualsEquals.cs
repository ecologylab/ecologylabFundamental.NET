using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    /// Annotation which describes which equals operator is to be used to comapre objects 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimplUseEqualsEquals : Attribute
    {

    }
}
