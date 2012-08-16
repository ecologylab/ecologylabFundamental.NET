using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Annotation describes a class field as a collection. 
    ///     This includes Lists which can be both mono-morphic and 
    ///     poly-morphic
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimplMap : Attribute
    {
        private readonly String _tagName;

         /// <summary>
        /// 
        /// </summary>
        private SimplMap()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        public SimplMap(String tagName)
        {
            this._tagName = tagName;
        }

        /// <summary>
        /// 
        /// </summary>
        public String TagName
        {
            get
            {
                return _tagName;
            }
        }
    }
}
