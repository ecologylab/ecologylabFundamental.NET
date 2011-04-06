using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.generic
{
    public interface IDictionaryList
    {
        void Add(object key, object value);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class DictionaryList<K, V> : Dictionary<K, V>, IDictionaryList
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

        public void Add(object key, object value)
        {
            this.Add((K) key, (V) value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        new public void Add(K key, V value)
        {
            this.Remove(key);
            
            base.Add(key, value);
            arrayList.Add(value);

            //return oldValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new public bool Remove(K key)
        {
            V oldValue = default(V);
            
            if (base.TryGetValue(key, out oldValue))
            {
                base.Remove(key);
                arrayList.Remove(oldValue);
                return true;
            }

            return false;
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

        public List<V> Values
        {
            get { return arrayList; }
        }

        public void PutAll(DictionaryList<K, V> otherList)
        {
            foreach(K key in otherList.Keys)
            {
                this.Add(key, otherList[key]);
            }
        }

    }
}
