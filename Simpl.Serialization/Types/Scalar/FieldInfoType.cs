using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    public class FieldInfoType : ReferenceType
    {
        public FieldInfoType() 
            : base(typeof(FieldInfo), CLTypeConstants.JavaField, CLTypeConstants.ObjCField, null)
        {
        }

        public override object GetInstance(string value, string[] formatStrings, IScalarUnmarshallingContext unmarshallingContext)
        {
            throw new NotImplementedException("Cannot instantate a Field type");
        }
    }
}
