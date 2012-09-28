using System;
using System.Collections.Generic;

// ReSharper disable CheckNamespace
namespace Ecologylab.Collections
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Scope<T> : Dictionary<String , T>
    {
        /// <summary>
        /// 
        /// </summary>
        public Scope()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public Scope(IDictionary<String, T> parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="size"></param>
        public Scope(IDictionary<String, T> parent, int size)
            : base(size)
        {
            Parent = parent;
        }

        public IDictionary<string, T> Parent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get(String name)
        {
            T result;
            if (TryGetValue(name, out result))
                return result;
            
            if (Parent != null && Parent.TryGetValue(name, out result))
                return result;
            return default(T);
        }

        public T Put(String key, T value)
        {
            T lastValue;

            if (TryGetValue(key, out lastValue))
                Remove(key);
              
            Add(key, value);

            return lastValue;
        }
    }
}
