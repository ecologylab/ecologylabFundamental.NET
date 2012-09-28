using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.OODSS.Messages;

namespace Simpl.OODSS.Distributed.Client
{
    public class RequestQueueObject
    {
        public RequestMessage RequestMessage { get; private set; }
        public long Uid { get; private set; }
        public TaskCompletionSource<ResponseMessage> Tcs { get; private set; }

        public RequestQueueObject(RequestMessage requestMessage, long uid, TaskCompletionSource<ResponseMessage> tcs)
        {
            RequestMessage = requestMessage;
            Uid = uid;
            Tcs = tcs;
        }
    }

    public class ResponseQueueObject
    {
        public ResponseMessage ResponseMessage { get; private set; }
        public long Uid { get; private set; }
        public ResponseQueueObject(ResponseMessage responseMessage, long uid)
        {
            ResponseMessage = responseMessage;
            Uid = uid;
        }
    }
}
