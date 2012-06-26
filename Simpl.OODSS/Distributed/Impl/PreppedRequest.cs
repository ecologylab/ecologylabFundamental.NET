using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Distributed.Impl
{
    public class PreppedRequest:IComparable<PreppedRequest>
    {
        public long Uid { get; set; }

        public StringBuilder Request { get; set; }

        public bool Disposable { get; set; }

        public PreppedRequest(int requestSize)
        {
            Request = new StringBuilder(requestSize);
        }

        public PreppedRequest(StringBuilder request, long uid, int requestSize, bool disposable)
        :this(requestSize)
        {
            Uid = uid;
            Request = request;
            Disposable = disposable;
        }

        public void Clear()
        {
            Uid = -1;
            Request = default(StringBuilder);
            Disposable = false;
        }

        public int CompareTo(PreppedRequest other)
        {
            return (int) (Uid - other.Uid);
        }
    }
}
