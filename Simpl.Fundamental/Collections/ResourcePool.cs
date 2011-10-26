using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Fundamental.Collections
{
    abstract class ResourcePool<T> where T : new()
    {
        private readonly List<T> _pool;

        private int _initialCapacity;

        protected ResourcePool(int initialCapacity)
        {
            _initialCapacity = initialCapacity;
            _pool = new List<T>(initialCapacity);
        }

        public T Acquire()
        {
            if (_pool.Count <= 0)
            {
                T resource = GenerateNewResource();
                return resource;
            }
            else
            {
                lock(_pool)
                {
                    T resource = _pool.ElementAt(_pool.Count - 1);
                    _pool.RemoveAt(_pool.Count - 1);
                    return resource;
                }
            }
        }

        public void Release(T resource)
        {
            lock(_pool)
            {
                _pool.Add(resource);
            }
        }

        public void Clear()
        {
            lock(_pool)
            {
                _pool.Clear();
            }
        }

        protected abstract T GenerateNewResource();

    }
}
