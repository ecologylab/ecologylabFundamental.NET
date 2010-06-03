using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.xml
{
    public class FieldTypes
    {
        public const int UNSET_TYPE = -999;
        /**
         * This means we experienced an error while parsing.
         * This should never happen.
         */
        public const int BAD_FIELD = -99;

        public const int NAMESPACE_IGNORED_ELEMENT = 0;

        public const int ATTRIBUTE = 1;

        public const int IGNORED_ATTRIBUTE = -ATTRIBUTE;

        public const int LEAF = 2;

        public const int NESTED_ELEMENT = 3;

        /**
         * This means that we don't bother to parse the element,
         * because the programmer developing ElementState subclasses did not
         * bother to create fields that use it.
         */
        public const int IGNORED_ELEMENT = -NESTED_ELEMENT;

        public const int COLLECTION_ELEMENT = 4;

        public const int COLLECTION_SCALAR = 5;

        public const int MAP_ELEMENT = 6;

        public const int MAP_SCALAR = 7;

        /**
         * Root node type
         */
        //TODO -- should this be merged with PSEUDO_FIELD_DESCRIPTOR
        public const int ROOT = 8;

        public const int TEXT_ELEMENT = 9;


        /**
         * This undocumented feature forms an ElementState subclass instance using lookup by class,
         * if that works, and then calls addNestedElement(), which the user can override, to decide what
         * do to with it.
         */
        public const int AWFUL_OLD_NESTED_ELEMENT = 99;


        public const int WRAPPER = 0x0a;

        public const int TEXT_NODE_VALUE = 0x0c;

        public const int PSEUDO_FIELD_DESCRIPTOR = 0x0d;

        public const int XMLNS_ATTRIBUTE = 0x0e;

        public const int XMLNS_IGNORED = 0x0f;

        public const int NAME_SPACE_MASK = 0x10;

        public const int NAMESPACE_TRIAL_ELEMENT = NAME_SPACE_MASK;

        public const int NAME_SPACE_ATTRIBUTE = NAME_SPACE_MASK + ATTRIBUTE;

        public const int NAME_SPACE_NESTED_ELEMENT = NAME_SPACE_MASK + NESTED_ELEMENT;

        public const int NAME_SPACE_LEAF_NODE = NAME_SPACE_MASK + LEAF;
    }
    
}
