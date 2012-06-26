using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Collections;

namespace Simpl.OODSS.Distributed.Impl
{
    public class PreppedRequestPool : ResourcePoolWithSize<PreppedRequest>
    {
        public PreppedRequestPool(int initialCapacity, int resourceObjectCapacity) : base(initialCapacity, resourceObjectCapacity)
        {
        }

        protected void Clean(PreppedRequest objectToClean)
        {
            objectToClean.Clear();
        }

        protected override PreppedRequest GenerateNewResource()
        {
            return new PreppedRequest(ResourceObjectCapacity);
        }
    }
}
