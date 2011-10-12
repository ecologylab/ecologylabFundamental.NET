using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Annotation which defines other tags in serialized form
    ///     This is used to support old XML files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SimplOtherTags : Attribute
    {
        private String[] _otherTags;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherTags"></param>
        public SimplOtherTags(String[] otherTags)
        {
            this._otherTags = otherTags;
        }

        /// <summary>
        /// 
        /// </summary>
        public String[] OtherTags
        {
            get
            {
                return _otherTags;
            }
            set
            {
                _otherTags = value;
            }
        }
    }
}
