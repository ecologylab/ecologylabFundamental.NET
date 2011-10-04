using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Types.Scalar;

namespace Simpl.Serialization.Types
{
    class FundamentalTypes
    {
        public static ScalarType stringType = new StringType();
        public static ScalarType intType = new IntType();
        public static ScalarType booleanType = new BooleanType();
        public static ScalarType doubleType = new DoubleType();
        public static ScalarType parsedUriType = new ParsedUriType();
        public static ScalarType floatType = new FloatType();

        public static CollectionType listType = new CollectionType(typeof (List<>), CLTypeConstants.JavaArraylist,
                                                                   CLTypeConstants.ObjCArraylist);

        public static CollectionType mapType = new CollectionType(typeof(Dictionary<,>), CLTypeConstants.JavaDictionary, CLTypeConstants.ObjCDictionary);

        static FundamentalTypes()
        {
            TypeRegistry.SetDefaultCollectionType(listType);
            TypeRegistry.SetDefaultMapType(mapType);
        }
    }
}
