using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Annotation which describes a class whose super classes also
    ///     contains the annotation meta-language which needs to be resolved. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SimplInherit : Attribute
    {

    }
}
