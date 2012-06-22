using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Collections;

namespace Simpl.OODSS.Distributed.Impl
{
    class MessageWithMetadataPool:ResourcePool<MessageWithMetadataPool>
    {
        public MessageWithMetadataPool(int initialCapacity) : base(initialCapacity)
        {
        }

        protected override MessageWithMetadataPool GenerateNewResource()
        {
            throw new NotImplementedException();
        }
    }
}
