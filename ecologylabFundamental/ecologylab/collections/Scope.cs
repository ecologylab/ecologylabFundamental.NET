using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Scope<T> : Dictionary<String , T>
    {
        private IDictionary<String, T> parent;

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
            this.parent = parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="size"></param>
        public Scope(IDictionary<String, T> parent, int size)
            : base(size)
        {
            this.parent = parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get(String name)
        {
            T result = default(T);
            if (base.TryGetValue(name, out result))
                return result;
            else
                return default(T);
        }
    }
}
