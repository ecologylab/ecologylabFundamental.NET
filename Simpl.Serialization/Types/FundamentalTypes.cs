using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Types.Scalar;

namespace Simpl.Serialization.Types
{
    public class FundamentalTypes
    {

        public static List<ScalarType> ScalarTypes
        {
            get
            {
                return new List<ScalarType>
                {
                            new StringType()
                            ,new IntType()
                            ,new BooleanType()
                            ,new DoubleType()
                            ,new ParsedUriType()
                            ,new FloatType()
                            ,new EnumeratedType()
                            ,new LongType()
                            ,new RegexType()
                            ,new StringBuilderType()
                            ,new DateTimeType()
                            ,new FileType()
                            ,new FieldInfoType()
                            ,new TypeType()
                            ,new ScalarTypeType()
                            ,new RectType()
                            ,new BinaryDataType()
                };
            }
        }

        private static CollectionType listType = new CollectionType(
                                                                    typeof(List<>),
                                                                    CLTypeConstants.JavaArraylist,
                                                                    CLTypeConstants.ObjCArraylist);

        private static CollectionType mapType = new CollectionType(
                                                                    typeof(Dictionary<,>),
                                                                    CLTypeConstants.JavaDictionary,
                                                                    CLTypeConstants.ObjCDictionary);

        private static CollectionType mapListType = new CollectionType(
                                                                    typeof(DictionaryList<,>),
                                                                    CLTypeConstants.JavaDictionaryarraylist,
                                                                    CLTypeConstants.ObjCDictionaryarraylist);


        private static List<CollectionType> collectionTypes = new List<CollectionType> { listType, mapListType, mapType };


        static FundamentalTypes()
        {
            TypeRegistry.RegisterTypes(ScalarTypes.ToArray());
            TypeRegistry.RegisterTypes(collectionTypes.ToArray());

            TypeRegistry.SetDefaultCollectionType(listType);
            TypeRegistry.SetDefaultMapType(mapType);
        }
    }
}
