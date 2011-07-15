using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;

namespace ecologylabFundamental.ecologylab.collections
{
    /// <summary>
    /// A collection type to hold a map of multiple elementstate objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="S"></typeparam>
    class MultiMap<T, S > where S : ElementState
    {
        private Dictionary<T, List<S>> map;
        
        /// <summary>
        /// Ddefault constructor
        /// </summary>
        public MultiMap()
        {
            map = new Dictionary<T,List<S>>();
        }

        /// <summary>
        /// Adds the given value pair into the map
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(T key, S value)
        {
            if (!map.ContainsKey(key))
            {
                List<S> collection = new List<S>(1);
                collection.Add(value);
                map.Add(key, collection);
                return true;
            }
            else
            {
                List<S> collection = map[key];
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
        public bool Contains(T key, S value)
        {
            if (map.ContainsKey(key))
            {
                List<S> collection = map[key];
                return ContainsValue(collection,value);                
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the size of the colelction
        /// </summary>
        public int Count
        {
            get { return map.Count; }
        }

        /// <summary>
        /// Returns whether the given value is included in the given collection based on the equals operator
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ContainsValue(List<S> collection, S value)
	    {
		    if(value.StrictObjectGraphRequired)
		    {
			    foreach(S item in collection)
			    {
				    if(item == value)
				    {
					    return true;
				    }
			    }
			    return false;
		    }else
		    {
			    foreach(S item in collection)
			    {
				    if(item.Equals(value))
				    {
					    return true;
				    }
			    }
			    return false;
		    }
	    }	
    }
}
