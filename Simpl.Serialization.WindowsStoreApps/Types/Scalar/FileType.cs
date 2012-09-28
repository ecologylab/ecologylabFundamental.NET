using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Types.Scalar
{
    class FileType : FileTypeBase
    {
        public new static readonly DateTime DefaultValue = DateTime.Now;
        public new const String DefaultValueString = "0";

        /// <summary>
        ///      Calls the parent constructor for int type
        /// </summary>
        public FileType()
            : this(typeof(StorageFile))
        {
        }

        public FileType(Type type)
            : base(type, CLTypeConstants.JavaFile, CLTypeConstants.ObjCFile, null)
        {
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatStrings"></param>
        /// <param name="scalarUnmarshallingContext"></param>
        /// <returns></returns>
        public override object GetInstance(String value, String[] formatStrings,
                                           IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            return StorageFile.GetFileFromPathAsync(value).GetResults();
        }

        public override string Marshall(object instance, TranslationContext context = null)
        {
            return ((StorageFile)instance).Name;
        }

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

    }
}
