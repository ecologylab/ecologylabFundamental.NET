using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml
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
        public const int NAMESPACE_IGNORED_ELEMENT = 0;

        /// <summary>
        /// 
        /// </summary>
        public const int ATTRIBUTE = 1;

        /// <summary>
        /// 
        /// </summary>
        public const int IGNORED_ATTRIBUTE = -ATTRIBUTE;

        /// <summary>
        /// 
        /// </summary>
        public const int LEAF = 2;

        /// <summary>
        /// 
        /// </summary>
        public const int NESTED_ELEMENT = 3;

        /// <summary>
        /// 
        /// </summary>
        public const int IGNORED_ELEMENT = -NESTED_ELEMENT;

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
        public const int ROOT = 8;

        /// <summary>
        /// 
        /// </summary>
        public const int TEXT_ELEMENT = 9;

        /// <summary>
        /// 
        /// </summary>
        public const int AWFUL_OLD_NESTED_ELEMENT = 99;

        /// <summary>
        /// 
        /// </summary>
        public const int WRAPPER = 0x0a;

        /// <summary>
        /// 
        /// </summary>
        public const int TEXT_NODE_VALUE = 0x0c;

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
        public const int NAME_SPACE_ATTRIBUTE = NAME_SPACE_MASK + ATTRIBUTE;

        /// <summary>
        /// 
        /// </summary>
        public const int NAME_SPACE_NESTED_ELEMENT = NAME_SPACE_MASK + NESTED_ELEMENT;

        /// <summary>
        /// 
        /// </summary>
        public const int NAME_SPACE_LEAF_NODE = NAME_SPACE_MASK + LEAF;
    }
}
