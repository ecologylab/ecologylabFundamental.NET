using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.generic
{
    class DictionaryList<K, V> : Dictionary<K, V>
    {
        protected List<V> arrayList;

        public DictionaryList()
        {
            arrayList = new List<V>();
        }

        public DictionaryList(int capacity) : base(capacity)
        {
            arrayList = new List<V>(capacity);
        }

        new public V Add(K key, V value)
        {
            V oldValue = this.Remove(key);
            
            base.Add(key, value);
            arrayList.Add(value);

            return oldValue;
        }

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

        new public void Clear()
        {
            arrayList.Clear();
            base.Clear();
        }

        public V ElementAt(int index)
        {
            return arrayList.ElementAt(index);
        }
    }
}
