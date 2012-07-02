using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Defines a field is represented as XML leaf when marshalled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimplScalar : Attribute
    {
       
    }
}