using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Distributed.Impl;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Client
{
    public class WebSocketOODSSClient:BaseClient
    {
        #region Data Member

        protected string ServerAddress;

        protected Scope<object> OjectRegistry;

        protected readonly StringBuilder RequestBuffer;
        protected readonly StringBuilder IncomingMessageBuffer;
        protected readonly StringBuilder OutgoingMessageBuffer;
        protected readonly StringBuilder OutgoingMessageHeaderBuffer;

        private readonly StringBuilder _currentHeaderSequence = new StringBuilder();
        private readonly StringBuilder _currentKeyHeaderSequence = new StringBuilder();

        private MessageWithMetadata<ServiceMessage, object> _response = null;
        private volatile bool _blockingRequestPending = false;        
        private readonly BlockingCollection<MessageWithMetadata<ServiceMessage,object>>  
            _blockingMessageWithMetadataQueue = new BlockingCollection<MessageWithMetadata<ServiceMessage, object>>();
        private BlockingCollection<ResponseMessage> 
            _blockingResponseMessageQueue = new BlockingCollection<ResponseMessage>(); 
        protected BlockingCollection<PreppedRequest>
            BlockingPreppedRequestQueue = new BlockingCollection<PreppedRequest>();
        protected readonly Dictionary<long, PreppedRequest> UnfulfilledRequests = new Dictionary<long, PreppedRequest>();

        protected int ReconnectAttemps = ClientConstants.ReconnectAttempts;
        protected int WaiteBetweenReconnectAttemps = ClientConstants.WaitBetweenReconnectAttempts;

        private string _sessionId;

        protected ReconnectedBlocker Blocker;

        protected long Selectinterval = 0;

        protected bool IsSending = false;

        private long _uidIndex = 1;
        private int _endOfFirstHeader = -1;
        protected int StartReadIndex = 0;
        private int _uidOfCurrentMessage = -1;
        private int _contentLengthRemaining = -1;

        private readonly StringBuilder _firstMessageBuffer = new StringBuilder();

        private bool _allowCompression = false;
        private bool _sendCompression = false;

        protected readonly Dictionary<string, string> HeaderMap = new Dictionary<string, string>();

        protected Socket thisSocket = null;

        protected readonly PreppedRequestPool PRequestPool;

        protected readonly MessageWithMetadataPool ResponsePool = new MessageWithMetadataPool(2);

        private readonly StringBuilderPool _builderPool;

        private int _maxMessageLengthChars;

        private string _contentEncoding;

        private SimplTypesScope _translationScope;



        #endregion

        public void SendMessage(RequestMessage request)
        {
            throw new NotImplementedException();
        }

        private void ProcessUpdate(UpdateMessage message)
        {
            message.ProcessUpdate(OjectRegistry);
        }
    }
}
