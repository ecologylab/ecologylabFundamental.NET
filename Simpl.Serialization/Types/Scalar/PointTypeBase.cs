using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    public abstract class PointTypeBase : ReferenceType
    {
        protected PointTypeBase(Type type, string javaTypeName, string objCTypeName, string dbTypeName)
            : base(type, javaTypeName, objCTypeName, dbTypeName)
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
