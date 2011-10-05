using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;
using Simpl.Serialization.Types;

namespace Simpl.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DescriptorBase : SimplBaseType
    {
        /// <summary>
        /// 
        /// </summary>
        [SimplScalar] 
        protected String tagName;

        /// <summary>
        /// 
        /// </summary>
        [SimplNoWrap] 
        [SimplCollection("other_tag")] 
        protected List<String> otherTags;
        
        /// <summary>
        /// 
        /// </summary>
        [SimplScalar] protected String comment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="name"></param>
        protected DescriptorBase(String tagName, String name)
            : this(tagName, name, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="name"></param>
        /// <param name="comment"></param>
        protected DescriptorBase(String tagName, String name, String comment)
            : base(name)
        {
            this.comment = comment;
            this.tagName = tagName;
        }

        /// <summary>
        /// 
        /// </summary>
        public String TagName { get { return tagName; } }

        /// <summary>
        /// 
        /// </summary>
        public abstract List<String> OtherTags { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherTag"></param>
        protected void AddOtherTag(String otherTag)
        {
            if (otherTags == null)
                otherTags = new List<String>();
            if (!otherTags.Contains(otherTag))
                otherTags.Add(otherTag);
        }

        /// <summary>
        /// 
        /// </summary>
        public String Comment
        {
            get { return comment; }
        }
    }
}
