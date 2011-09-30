using System;
using System.Collections.Generic;
using System.Linq;
using ecologylab.serialization;

namespace Simpl.Serialization.Graph
{
    /// <summary>
    /// A collection type to hold a map of multiple elementstate objects
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    class MultiMap<TK>
    {
        private readonly Dictionary<TK, List<Object>> _map;
        
        /// <summary>
        /// Ddefault constructor
        /// </summary>
        public MultiMap()
        {
            _map = new Dictionary<TK, List<Object>>();
        }

        /// <summary>
        /// Adds the given value pair into the map
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(TK key, Object value)
        {
            if (!_map.ContainsKey(key))
            {
                var collection = new List<Object>(1) { value };
                _map.Add(key, collection);
                return true;
            }
            else
            {
                List<Object> collection = _map[key];
                if (!ContainsValue(collection,value))
                {
                    collection.Add(value);
                    return true;
                }                
            }
            return false;
        }

        /// <summary>
        /// Returns whether the given key , value pair is present in the map
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(TK key, Object value)
        {
            if (_map.ContainsKey(key))
            {
                List<Object> collection = _map[key];
                return ContainsValue(collection,value);                
            }
            return false;
        }

        /// <summary>
        /// Returns the size of the colelction
        /// </summary>
        public int Count
        {
            get { return _map.Count; }
        }

        /// <summary>
        /// Returns whether the given value is included in the given collection based on the equals operator
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ContainsValue(IEnumerable<Object> collection, Object value)
        {
            ClassDescriptor classDescriptor = ClassDescriptor.GetClassDescriptor(value.GetType());
            if (classDescriptor.StrictObjectGraphRequired)
		    {
                return collection.Any(item => item == value);
		    }
            return collection.Contains(value);
        }
    }
}
