using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Annotation which describes the collection as wrapped 
    ///     collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimplWrap : Attribute
    {

    }
}
