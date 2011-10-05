using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Defines a new tag on a field. By default the field name is taken as 
    ///     the attribute of the field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SimplTag : Attribute
    {
        private readonly String _tagName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        public SimplTag(String tagName)
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