using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    public abstract class FileTypeBase : ReferenceType
    {
        public new static readonly DateTime DefaultValue = DateTime.Now;
        public new const String DefaultValueString = "0";

        protected FileTypeBase(Type thatClass, string javaTypeName, string objectiveCTypeName, string dbTypeName) 
            : base(thatClass, javaTypeName, objectiveCTypeName, dbTypeName)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns></returns>
        public abstract override object GetInstance(String value, String[] formatStrings,
                                           IScalarUnmarshallingContext scalarUnmarshallingContext);

        public abstract override string Marshall(object instance, TranslationContext context = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsDefaultValue(FieldInfo field, Object context)
        {
            return field.GetValue(context).ToString() == DefaultValueString;
        }


        public override bool SimplEquals(object left, object right)
        {
            // TODO: need platform specifics comparison
//            if (left is FileInfo && right is FileInfo)
//            {
//                var leftFile = left as FileInfo;
//                var rightFile = right as FileInfo;
//
//                return base.GenericSimplEquals<string>(leftFile.FullName, rightFile.FullName);
//            }
//            else
//            {
//                return false;
//            }
            return true;
        }
    }
}
