using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     An array of classes used to bind the type of classes 
    ///     the annotated attribute can hold. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SimplClasses : Attribute
    {
        private readonly Type[] _classes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classes"></param>
        public SimplClasses(Type[] classes)
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
