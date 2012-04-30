using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Types.Scalar;

namespace Simpl.Serialization.Types
{
    class FundamentalTypes
    {
        public static ScalarType stringType         = new StringType();
        public static ScalarType intType            = new IntType();
        public static ScalarType booleanType        = new BooleanType();
        public static ScalarType doubleType         = new DoubleType();
        public static ScalarType parsedUriType      = new ParsedUriType();
        public static ScalarType floatType          = new FloatType();
        public static ScalarType enumeratedType     = new EnumeratedType();
        public static ScalarType longType           = new LongType();
        public static ScalarType regexType          = new RegexType();
        public static ScalarType stringBuilderType  = new StringBuilderType();
        public static ScalarType dateTimeType       = new DateTimeType();
        public static ScalarType fileType           = new FileType();
        public static ScalarType fieldInfoType      = new FieldInfoType();
        public static ScalarType typeType           = new TypeType();
        public static ScalarType scalarTypeType     = new ScalarTypeType();
        public static ScalarType rectType           = new RectType();


        public static CollectionType listType       = new CollectionType(
                                                                    typeof (List<>),
                                                                    CLTypeConstants.JavaArraylist,
                                                                    CLTypeConstants.ObjCArraylist);

        public static CollectionType mapType        = new CollectionType(
                                                                    typeof(Dictionary<,>),
                                                                    CLTypeConstants.JavaDictionary,
                                                                    CLTypeConstants.ObjCDictionary);

        public static CollectionType mapListType = new CollectionType(
                                                                    typeof(DictionaryList<,>),
                                                                    CLTypeConstants.JavaDictionaryarraylist,
                                                                    CLTypeConstants.ObjCDictionaryarraylist);


        static FundamentalTypes()
        {
            TypeRegistry.SetDefaultCollectionType(listType);
            TypeRegistry.SetDefaultMapType(mapType);
        }
    }
}
