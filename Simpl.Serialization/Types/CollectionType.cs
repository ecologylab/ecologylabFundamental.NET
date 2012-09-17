namespace Simpl.Serialization.Types
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Simpl.Serialization.Attributes;
    using System.Linq;
    
    /// <summary>
    /// Basic cross-platform unit for managing Collection and Map types in S.IM.PL Serialization.
    /// </summary>
    public class CollectionType : SimplType
    {
        /// <summary>
        /// Determines if a simpl Collection Type can be created for the given C# TYpe
        /// </summary>
        /// <param name="aType">The type to consider</param>
        /// <returns>True if a collection type can be made</returns>
        public static bool CanBeCreatedFrom(Type aType)
        {
            return aType.IsAssignableTo<IList>() || aType.IsAssignableTo<IDictionary>();
        }

        /// <summary>
        /// 
        /// </summary>
        [SimplScalar] 
        private readonly Boolean _isDictionary;

        [SimplScalar]
        private readonly Boolean _isList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cSharpType"></param>
        /// <param name="javaName"></param>
        /// <param name="objCName"></param>
        public CollectionType(Type cSharpType, String javaName, String objCName)
            : base(cSharpType, false, javaName, objCName, null)
        {
            _isDictionary = cSharpType.IsAssignableTo<IDictionary>();
            _isList = cSharpType.IsAssignableTo<IList>();

            if (_isDictionary == false && _isList == false)
            {
                throw new ArgumentException(String.Format("Collection type of {0} is not currently supported!", cSharpType.Name));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsDictionary
        {
            get { return _isDictionary; }
        }

        /// <summary>
        /// Gets a valid indicating wether or not this CollectionType is a list.
        /// </summary>
        public Boolean IsList
        {
            get
            {
                return _isList;
            }
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

        private IEnumerable<object> SortByHashCode(ICollection collection)
        {
            return collection.Cast<object>().OrderBy(obj => obj.GetHashCode());
        }

        public override bool SimplEquals(object left, object right)
        {
            if (this.IsDictionary)
            {
                if (left is IDictionary && right is IDictionary)
                {
                    var leftDict = left as IDictionary;
                    var rightDict = right as IDictionary;

                    if (leftDict.Keys.Count == rightDict.Keys.Count)
                    {
                        var v = new SimplObjectEqualityComparer();

                        foreach (var key in leftDict.Keys)
                        {
                            if (!rightDict.Contains(key))
                            {
                                return false; //mismatched key, not equal.
                            }
                            else
                            {
                                // We a collection, obtain the relevant collection type and compare. 
                                if (leftDict[key] is IList || leftDict[key] is IDictionary)
                                {
                                    var ct = TypeRegistry.CollectionTypes[leftDict[key].GetType()];

                                    if (!ct.SimplEquals(leftDict[key], rightDict[key]))
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    // Compare the internal values with our comparator
                                    if (!v.Equals(leftDict[key], rightDict[key]))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        // We've gotten through all wrong cases, must be equal.
                        return true; 
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new InvalidOperationException(String.Format("LHS and RHS are not IDicionary types. LHS: {0}, RHS: {1}", left.GetType().Name, right.GetType().Name));
                }

            }
            else if (this.IsList)
            {
                if (left is IList && right is IList)
                {
                    var leftList = left as IList;
                    var rightList = right as IList;

                    return Enumerable.SequenceEqual<object>(leftList.Cast<object>(), rightList.Cast<object>(), new SimplObjectEqualityComparer());                  
                }
                else
                {
                    throw new InvalidOperationException(String.Format("LHS and RHS are not IList types. LHS: {0}, RHS: {1}", left.GetType().Name, right.GetType().Name));
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format("Invalid collection type was used for SimplEquals at CollectionType. Type lhs:{0}, rhs{1}", left.GetType().Name, right.GetType().Name));
            }
        }
    }

    /// <summary>
    /// Compares two Simpl ILists, handles inner comparison of scalars, collections, or composites. 
    /// </summary>
    public class SimplObjectEqualityComparer : IEqualityComparer<object>
    {
        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public new bool Equals(object x, object y)
        {
            if (x.GetType().Equals(y.GetType()))
            {
                if(TypeRegistry.ScalarTypes.Contains(x.GetType()))
                {
                    return TypeRegistry.ScalarTypes[x.GetType()].SimplEquals(x,y);
                }
                else if (CollectionType.CanBeCreatedFrom(x.GetType()))
                {
                    var collectionType = TypeRegistry.CollectionTypes.GetOrAdd(x.GetType());
                    return collectionType.SimplEquals(x, y);
                }
                else
                {
                    var compositeType =  TypeRegistry.CompositeTypes.GetOrAdd(x.GetType());
                    return compositeType.SimplEquals(x, y);
                }
            }
            else
            {
                return false;
            }
        }
    }
}
