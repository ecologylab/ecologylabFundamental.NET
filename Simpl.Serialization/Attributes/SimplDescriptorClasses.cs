using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     An array of classes used to bind the type of classes 
    ///     the annotated attribute can hold. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SimplDescriptorClasses : Attribute
    {
        private readonly Type[] _classes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classes"></param>
        public SimplDescriptorClasses(Type[] classes)
        {
            this._classes = classes;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type[] Classes
        {
            get
            {
                return _classes;
            }
        }
    }
}
