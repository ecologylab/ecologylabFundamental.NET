using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.OODSS.Messages;

namespace Simpl.OODSS.Distributed.Impl
{
    class MessageWithMetadata<M, A> : IComparable<MessageWithMetadata<M, A>> where M:ServiceMessage
    {
        private long _uid;

        private M _message;
        private A _attachment;

        public long Uid
        {
            get { return _uid; }
            set { _uid = value; } 
        }
        public M Message
        {
            get { return _message; }
            set { _message = value; }
        }
        public A Attachment
        {
            get { return _attachment; }
            set { _attachment = value; } 
        }

        public MessageWithMetadata(M response, long uid=-1, A attachment=default(A))
        {
            Uid = uid;
            Attachment = attachment;
            Message = response;
        }

        public int CompareTo(MessageWithMetadata<M, A> other)
        {
            return (int) (_uid - other._uid);
        }

        public void Clear()
        {
            Uid = -1;
            Message = null;
            Attachment = default(A);
        }


    }
}
