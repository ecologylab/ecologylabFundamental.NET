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
        private readonly String _tagName;

        /// <summary>
        /// 
        /// </summary>
        public SimplComposite()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        public SimplComposite(String tagName)
        {
            this._tagName = tagName;
        }

        /// <summary>
        /// 
        /// </summary>
        public String TagName
        {
            get { return _tagName; }
        }
    }
}
