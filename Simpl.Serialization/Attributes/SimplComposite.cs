using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Annotation describes a field which is composed
    ///     of further fields inside which are also serializable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SimplComposite : Attribute
    {

    }
}
