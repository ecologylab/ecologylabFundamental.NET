using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Annotation describes a collection as not wrapped
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimplNoWrap : Attribute
    {

    }
}
