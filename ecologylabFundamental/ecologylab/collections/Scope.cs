using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylabFundamental.ecologylab.collections
{
    class Scope<T> : Dictionary<String , T>
    {
        private IDictionary<String, T> parent;

        public Scope()
        {
        }

        public Scope(IDictionary<String, T> parent)
        {
            this.parent = parent;
        }

        public Scope(IDictionary<String, T> parent, int size)
            : base(size)
        {
            this.parent = parent;
        }

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
