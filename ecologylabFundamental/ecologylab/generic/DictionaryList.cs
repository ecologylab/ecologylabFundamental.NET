using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.generic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class DictionaryList<K, V> : Dictionary<K, V>
    {
        /// <summary>
        /// 
        /// </summary>
        protected List<V> arrayList;

        /// <summary>
        /// 
        /// </summary>
        public DictionaryList()
        {
            arrayList = new List<V>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public DictionaryList(int capacity) : base(capacity)
        {
            arrayList = new List<V>(capacity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        new public V Add(K key, V value)
        {
            V oldValue = this.Remove(key);
            
            base.Add(key, value);
            arrayList.Add(value);

            return oldValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new public V Remove(K key)
        {
            V oldValue = default(V);
            
            if (base.TryGetValue(key, out oldValue))
            {
                base.Remove(key);
                arrayList.Remove(oldValue);
            }

            return oldValue;
        }

        /// <summary>
        /// 
        /// </summary>
        new public void Clear()
        {
            arrayList.Clear();
            base.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public V ElementAt(int index)
        {
            return arrayList.ElementAt(index);
        }
    }
}
