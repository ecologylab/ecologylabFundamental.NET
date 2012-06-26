using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Fundamental.Collections
{
    /// <summary>
    ///  A scope (map: String -&gt; T) with multiple ancestors.
    /// 
    /// NOTE that currently this class uses a LRU cache to cache look-ups for values from ancestors for
    /// efficiency, thus removing a value from an ancestor may not work properly. if you need removing
    /// please set CACHE_SIZE to zero or modify the class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiAncestorScope<T> : Dictionary<string, T>
        where T : class 
    {
        /// <summary>
        /// Ancestors of this scope
        /// </summary>
        private List<Dictionary<string, T>> ancestors;

        private LRUCache<string, T> queryCache;

        public MultiAncestorScope(params Dictionary<string, T>[] ancestors)
        {
            AddAncestors(ancestors);
        }


        public T Get(string key)
        {
            var visited = new HashSet<Dictionary<string, T>>();
            return GetHelper(key, visited);
        }

        private T GetHelper(string key, HashSet<Dictionary<string, T>> visited)
        {
            T result = null;
            if (base.ContainsKey(key)) 
                TryGetValue(key, out result);
            
            if (result == null)
			    result = this.GetFromCache(key);
		    if (result == null)
		    {
			    if (this.ancestors != null)
				    foreach (var ancestor in this.ancestors)
					    if (containsSame(visited, ancestor))
						    continue;
					    else
					    {
						    visited.Add(ancestor);
						    if (ancestor is MultiAncestorScope<T>)
							    result = ((MultiAncestorScope<T>) ancestor).GetHelper(key, visited);
						    else
							    ancestor.TryGetValue(key, out result);
						    if (result != null)
						    {
							    this.PutToCache(key, result);
							    break;
						    }
					    }
		}
		return result;
        }

        ///<summary>
        /// only put value into the scope when it is not null. this prevents shadowing values with the
        /// same key in ancestors.
        ///</summary>
        public void AddIfValueNotNull(String key, T value)
        {
            if (value != null)
                Add(key, value);
        }

        private void PutToCache(string key, T value)
        {
            if(queryCache == null)
                queryCache = new LRUCache<string, T>();
            queryCache.Add(key, value);
        }

        private T GetFromCache(string key)
        {
            return queryCache == null ? null : queryCache.Get(key);
        }

        /**
         * get a List of values from this scope AND its ancestors. values ordered from near to far.
         * 
         * @param key
         * @return
         */
        public List<T> GetAll(Object key)
        {
            List<T> results = new List<T>();
            List<Dictionary<string, T>> visited = new List<Dictionary<string, T>>();
            GetAllHelper(key, visited, results);
            return results;
        }

	    private void GetAllHelper(Object key, List<Dictionary<string, T>> visited, List<T> results)
	    {
		    T result;
            base.TryGetValue(key.ToString(), out result);
		    if (result != null)
			    results.Add(result);
		    if (this.ancestors != null)
			    foreach (Dictionary<string, T> ancestor in this.ancestors)
				    if (containsSame(visited, ancestor))
					    continue;
				    else
				    {
					    visited.Add(ancestor);
					    if (ancestor is MultiAncestorScope<T>)
						    ((MultiAncestorScope<T>) ancestor).GetAllHelper(key, visited, results);
					    else
					    {
                            ancestor.TryGetValue(key.ToString(), out result);
						    if (result != null)
							    results.Add(result);
					    }
				    }
	    }

        /// <summary>
        /// Ancestors of this scope
        /// </summary>
        public List<Dictionary<string, T>> Ancestors
        {
            get { return ancestors ?? (ancestors = new List<Dictionary<string, T>>()); }
            set { ancestors = value; }
        }

        private void AddAncestors(IEnumerable<Dictionary<string, T>> additionalAncestors)
        {
            if(additionalAncestors != null)
                foreach (var ancestor in additionalAncestors)
                    AddAncestor(ancestor);
        }

        public void AddAncestor(Dictionary<string, T> ancestor)
        {
            if(ancestor == null || containsSame(Ancestors, ancestor))
                return;
            ancestors.Add(ancestor);
        }

        private static bool containsSame(IEnumerable<Dictionary<string, T>> list, Dictionary<string, T> ancestor)
        {
            return list.Any(a => a == ancestor);
        }
    }
}
