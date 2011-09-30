using System;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Types
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SimplBaseType
    {
        /// <summary>
        /// 
        /// </summary>
        [SimplScalar] 
        [SimplOtherTags(new string[] {"field_name", "described_class_name"})] 
        protected String name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected SimplBaseType(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>s
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract String JavaTypeName { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract String CSharpTypeName { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract String ObjectiveCTypeName { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract String DbTypeName { get; }
    }
}
