using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Collections;
using Simpl.OODSS.Messages;

namespace Simpl.OODSS.Distributed.Impl
{
    public class MessageWithMetadataPool:ResourcePool<MessageWithMetadata<ServiceMessage, object>>
    {
        public MessageWithMetadataPool(int initialCapacity) : base(initialCapacity)
        {
        }

        protected void Clean(MessageWithMetadata<ServiceMessage, object> objectToClean)
        {
            objectToClean.Clear();
        }

        protected override MessageWithMetadata<ServiceMessage, object> GenerateNewResource()
        {
            return new MessageWithMetadata<ServiceMessage, object>();
        }
    }
}
