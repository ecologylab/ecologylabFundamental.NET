using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.OODSS.Messages;

namespace Simpl.OODSS.Distributed.Impl
{
    class MessageWithMetadata<M, A> : IComparable<MessageWithMetadata<M, A>> where M:ServiceMessage
    {
        public int CompareTo(MessageWithMetadata<M, A> other)
        {
            throw new NotImplementedException();
        }
    }
}
