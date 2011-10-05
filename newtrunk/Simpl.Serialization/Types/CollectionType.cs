using System;
using System.Collections;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Types
{
    /// <summary>
    /// Basic cross-platform unit for managing Collection and Map types in S.IM.PL Serialization.
    /// </summary>
    public class CollectionType : SimplType
    {
        /// <summary>
        /// 
        /// </summary>
        [SimplScalar] 
        private readonly Boolean _isDictionary;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpType"></param>
        /// <param name="javaName"></param>
        /// <param name="objCName"></param>
        public CollectionType(Type cSharpType, String javaName, String objCName)
            : base(cSharpType, false, javaName, objCName, null)
        {
            _isDictionary = typeof (IDictionary).IsAssignableFrom(cSharpType);
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsDictionary
        {
            get { return _isDictionary; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary Dictionary
        {
            get { return _isDictionary ? (IDictionary) Instance : null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList List
        {
            get { return !_isDictionary ? (IList) Instance : null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string DeriveJavaTypeName()
        {
            return JavaTypeName ?? TypeRegistry.GetDefaultCollectionOrMapType(_isDictionary).JavaTypeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string DeriveObjectiveCTypeName()
        {
            return ObjectiveCTypeName ?? TypeRegistry.GetDefaultCollectionOrMapType(_isDictionary).ObjectiveCTypeName;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool IsScalar
        {
            get { return false; }
        }
    }
}
