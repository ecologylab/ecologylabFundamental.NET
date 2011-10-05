using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Annotation which describes the collection as wrapped 
    ///     collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SimplWrap : Attribute
    {

    }
}
