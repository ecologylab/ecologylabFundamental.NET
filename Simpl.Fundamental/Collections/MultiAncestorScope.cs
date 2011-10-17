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

        private void AddAncestor(Dictionary<string, T> ancestor)
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
