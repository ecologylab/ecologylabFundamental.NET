﻿using System;
using System.Reflection;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Types
{
    /// <summary>
    /// Represents a type mapping in Simpl, handling marshalling to and from a string representation, equality, and other tasks.
    /// </summary>
    [SimplInherit]
    public abstract class SimplType : SimplBaseType
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Type _cSharpType;

        /// <summary>
        /// 
        /// </summary>
        [SimplScalar]
        protected String simpleName;

        /// <summary>
        /// 
        /// </summary>
        [SimplScalar]
        protected String javaTypeName;

        /// <summary>
        /// 
        /// </summary>
        [SimplScalar]
        protected String cSharpTypeName;

        /// <summary>
        /// 
        /// </summary>
        [SimplScalar]
        protected String objectiveCTypeName;

        /// <summary>
        /// 
        /// </summary>
        [SimplScalar]
        protected String dbTypeName;

        /// <summary>
        /// 
        /// </summary>
        [SimplScalar]
        protected String nameSpaceName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpType"></param>
        /// <param name="isScalar"></param>
        /// <param name="javaTypeName"></param>
        /// <param name="objectiveCTypeName"></param>
        /// <param name="dbTypeName"></param>
        protected SimplType(Type cSharpType, Boolean isScalar, String javaTypeName, String objectiveCTypeName,
                            String dbTypeName) :
            this(
                                cSharpType.GetTypeInfo().IsPrimitive
                                    ? cSharpType.FullName
                                    : DeriveCrossPlatformName(cSharpType, isScalar), cSharpType, javaTypeName,
                                objectiveCTypeName, dbTypeName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cSharpType"></param>
        /// <param name="javaTypeName"></param>
        /// <param name="objectiveCTypeName"></param>
        /// <param name="dbTypeName"></param>
        protected SimplType(string name, Type cSharpType, String javaTypeName, String objectiveCTypeName,
                            String dbTypeName)
            : base(name)
        {
            _cSharpType = cSharpType;
            simpleName = javaTypeName;
            cSharpTypeName = cSharpType.Name;

            this.javaTypeName = javaTypeName;
            this.objectiveCTypeName = objectiveCTypeName;

            if (!cSharpType.GetTypeInfo().IsPrimitive)
                nameSpaceName = cSharpType.Namespace;

            this.dbTypeName = dbTypeName;

            TypeRegistry.RegisterSimplType(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public override String JavaTypeName
        {
            get { return javaTypeName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override String CSharpTypeName
        {
            get { return cSharpTypeName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override String ObjectiveCTypeName
        {
            get { return objectiveCTypeName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override String DbTypeName
        {
            get { return dbTypeName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String SimplName
        {
            get { return simpleName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type CSharpType
        {
            get { return _cSharpType; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract String DeriveJavaTypeName();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract String DeriveObjectiveCTypeName();

        /// <summary>
        /// 
        /// </summary>
        protected abstract Boolean IsScalar { get; }

        public Object Instance { get { return Activator.CreateInstance(_cSharpType); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpType"></param>
        /// <param name="isScalar"></param>
        /// <returns></returns>
        public static string DeriveCrossPlatformName(Type cSharpType, bool isScalar)
        {
            String cSharpTypeName = cSharpType.FullName;
            return cSharpTypeName != null && cSharpTypeName.StartsWith("cSharp") ? (isScalar ? CLTypeConstants.SimplScalarTypesPrefix : CLTypeConstants.SimplCollectionTypesPrefix) + cSharpType.Name : cSharpTypeName;
        }


        /// <summary>
        /// Determines if two objects represented by the SimplType are equivilant.
        /// </summary>
        /// <param name="left">Left hand side</param>
        /// <param name="right">Right hand side</param>
        /// <returns>True if the two are equal</returns>
        public abstract bool SimplEquals(object left, object right);

    }
}
