using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SimplMapKeyField : Attribute
    {
        private readonly String _fieldName;

        /// <summary>
        /// 
        /// </summary>
        public SimplMapKeyField()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        public SimplMapKeyField(String fieldName)
        {
            this._fieldName = fieldName;
        }

        /// <summary>
        /// 
        /// </summary>
        public String FieldName
        {
            get
            {
                return _fieldName;
            }
        }
    }
}
