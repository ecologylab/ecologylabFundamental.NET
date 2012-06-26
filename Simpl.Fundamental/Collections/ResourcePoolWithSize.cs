using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Fundamental.Collections
{
    public abstract class ResourcePoolWithSize<T>: ResourcePool<T>
    {
        protected int ResourceObjectCapacity;

        public ResourcePoolWithSize(int initialCapacity, int resourceObjectCapacity) : base(initialCapacity)
        {
            ResourceObjectCapacity = resourceObjectCapacity;
        }
    }
}
