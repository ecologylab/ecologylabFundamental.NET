using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace Simpl.Fundamental.Collections
{

    public class LRUCache<K, V>
    {
        private const int CACHE_SIZE = 16;
        private readonly int capacity;
        readonly Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap;
        readonly LinkedList<LRUCacheItem<K, V>> lruList;

        private readonly object _syncLock = new object();

        public LRUCache(int capacity = CACHE_SIZE)
        {
            lruList = new LinkedList<LRUCacheItem<K, V>>();
            cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
            this.capacity = capacity;
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public V Get(K key)
        {
            lock(_syncLock)
            {
                LinkedListNode<LRUCacheItem<K, V>> node;
                if (cacheMap.TryGetValue(key, out node))
                {
                    //System.Console.WriteLine("Cache HIT " + key);
                    V value = node.Value.value;

                    lruList.Remove(node);
                    lruList.AddLast(node);
                    return value;
                }
                //System.Console.WriteLine("Cache MISS " + key);
                return default(V);
            }
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(K key, V val)
        {
            lock(_syncLock)
            {
                if (cacheMap.Count >= capacity)
                {
                    removeFirst();
                }
                LRUCacheItem<K, V> cacheItem = new LRUCacheItem<K, V>(key, val);
                LinkedListNode<LRUCacheItem<K, V>> node = new LinkedListNode<LRUCacheItem<K, V>>(cacheItem);
                lruList.AddLast(node);
                cacheMap.Add(key, node);
            }
        }


        protected void removeFirst()
        {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem<K, V>> node = lruList.First;
            lruList.RemoveFirst();
            // Remove from cache
            cacheMap.Remove(node.Value.key);
        }


    }


    internal class LRUCacheItem<K, V>
    {
        public LRUCacheItem(K k, V v)
        {
            key = k;
            value = v;
        }
        public K key;
        public V value;
    }
}

