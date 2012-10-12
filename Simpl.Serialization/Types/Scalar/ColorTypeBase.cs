using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    public abstract class ColorTypeBase : ReferenceType
    {
        protected ColorTypeBase(Type thatClass, string javaTypeName, string objectiveCTypeName, string dbTypeName) : base(thatClass, javaTypeName, objectiveCTypeName, dbTypeName)
        {

        }

        public abstract override object GetInstance(String value, String[] formatStrings,
                                                    IScalarUnmarshallingContext scalarUnmarshallingContext);

        public abstract override string Marshall(object instance, TranslationContext context = null);

        public override bool SimplEquals(object left, object right)
        {
            return base.GenericSimplEquals<object>(left, right);
        }
    }
}
