using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.serialization
{
    /// <summary>
    ///     
    /// </summary>
    public class FieldTypes
    {
        /// <summary>
        /// 
        /// </summary>
        public const int UNSET_TYPE = -999;

        /// <summary>
        /// 
        /// </summary>
        public const int BAD_FIELD = -99;

        /// <summary>
        /// 
        /// </summary>
        public const int IGNORED_ATTRIBUTE = -1;

        /// <summary>
        /// 
        /// </summary>
        public const int SCALAR = 0x12;

        /// <summary>
        /// 
        /// </summary>
        public const int COMPOSITE_ELEMENT = 3;

        /// <summary>
        /// 
        /// </summary>
        public const int IGNORED_ELEMENT = -3;

        /// <summary>
        /// 
        /// </summary>
        public const int COLLECTION_ELEMENT = 4;

        /// <summary>
        /// 
        /// </summary>
        public const int COLLECTION_SCALAR = 5;

        /// <summary>
        /// 
        /// </summary>
        public const int MAP_ELEMENT = 6;

        /// <summary>
        /// 
        /// </summary>
        public const int MAP_SCALAR = 7;
        
        /// <summary>
        /// 
        /// </summary>
        public const int WRAPPER = 0x0a;

        /// <summary>
        /// 
        /// </summary>
        public const int PSEUDO_FIELD_DESCRIPTOR = 0x0d;

        /// <summary>
        /// 
        /// </summary>
        public const int XMLNS_ATTRIBUTE = 0x0e;

        /// <summary>
        /// 
        /// </summary>
        public const int XMLNS_IGNORED = 0x0f;

        /// <summary>
        /// 
        /// </summary>
        public const int NAME_SPACE_MASK = 0x10;

        /// <summary>
        /// 
        /// </summary>
        public const int NAMESPACE_TRIAL_ELEMENT = NAME_SPACE_MASK;

        /// <summary>
        /// 
        /// </summary>        
        public const int NAME_SPACE_ATTRIBUTE = NAME_SPACE_MASK + SCALAR;

        /// <summary>
        /// 
        /// </summary>
        public const int NAME_SPACE_NESTED_ELEMENT = NAME_SPACE_MASK + COMPOSITE_ELEMENT;

        /// <summary>
        /// 
        /// </summary>
        public const int NAME_SPACE_LEAF_NODE = NAME_SPACE_MASK + SCALAR;
    }
}