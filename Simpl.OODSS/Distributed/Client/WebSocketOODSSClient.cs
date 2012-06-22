using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Distributed.Impl;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using WebSocket4Net;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Client
{
    public class WebSocketOODSSClient : BaseClient
    {
        #region Data Member

        protected string ServerAddress;

        protected Scope<object> ObjectRegistry;

        protected readonly StringBuilder RequestBuffer;
        protected readonly StringBuilder IncomingMessageBuffer;
        protected readonly StringBuilder OutgoingMessageBuffer;
        protected readonly StringBuilder OutgoingMessageHeaderBuffer;

        private readonly StringBuilder _currentHeaderSequence = new StringBuilder();
        private readonly StringBuilder _currentKeyHeaderSequence = new StringBuilder();

        private MessageWithMetadata<ServiceMessage, object> _response = null;
        private volatile bool _blockingRequestPending = false;

        private readonly BlockingCollection<MessageWithMetadata<ServiceMessage, object>>
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

        private bool _firstMessageSent;
        private ElementState _lastElementState;

        #endregion

        #region WebSocketComponent

        private AutoResetEvent _messageReceiveEvent = new AutoResetEvent(false);
        private AutoResetEvent _dataReceiveEvent = new AutoResetEvent(false);
        private AutoResetEvent _openedEvent = new AutoResetEvent(false);
        private AutoResetEvent _closeEevnt = new AutoResetEvent(false);

        private WebSocket _webSocketClient;

        protected string CurrentMessage { get; private set; }
        protected byte[] CurrentData { get; private set; }

        #endregion WebSocketComponent

        #region Constructor

        public WebSocketOODSSClient(String uri, SimplTypesScope translationScope,
                                    WebSocketVersion version = WebSocketVersion.Rfc6455) : base()
        {
            _webSocketClient = new WebSocket(uri, "basic", version);
            _webSocketClient.Opened += webSocketClient_Opened;
            _webSocketClient.Closed += webSocketClient_Closed;
            _webSocketClient.DataReceived += webSocketClient_DataReceived;
            _webSocketClient.MessageReceived += webSocketClient_MessageReceived;

            _webSocketClient.Open();
            if (!_openedEvent.WaitOne(2000))
                Console.WriteLine("Handshake failed");
        }

        #endregion Constructor

        #region OODSS methods

        public bool Connect()
        {
            throw new NotImplementedException();
        }

        private bool ConnectImpl()
        {
            throw new NotImplementedException();
        }

        protected void Reconnect()
        {
        }



        //No blocking
        public void SendMessage(RequestMessage request)
        {
            throw new NotImplementedException();
        }

        private void ProcessUpdate(UpdateMessage updateMessage)
        {
            updateMessage.ProcessUpdate(ObjectRegistry);
        }

        private void ProcessResponse(ResponseMessage responseMessage)
        {
            responseMessage.ProcessResponse(ObjectRegistry);
        }

        private MessageWithMetadata<ServiceMessage, object> ProcessString(string incomingMessage, int incomingUid)
        {
            _response = TranslateStringToServiceMessage(incomingMessage, incomingUid);
            if (_response == null)
            {
                Console.WriteLine("Deserialized failed");
            }
            else
            {
                if(_response.Message is ResponseMessage)
                {
                    Console.WriteLine("Got a response message");
                    ProcessResponse((ResponseMessage) _response.Message);

                    // TODO: someting related to unfilfilledRequests blah
                }
                else if (_response.Message is UpdateMessage)
                {
                    Console.WriteLine("Got an updateMessage");
                    ProcessUpdate((UpdateMessage) _response.Message);
                }
            }
            return _response;
        }

        private MessageWithMetadata<ServiceMessage, object> TranslateStringToServiceMessage(string incomingMessage, int incomingUid)
        {
            ServiceMessage responseMessage = (ServiceMessage) _translationScope.Deserialize(incomingMessage, StringFormat.Json);
            if (responseMessage == null)
                return null;

            MessageWithMetadata<ServiceMessage, object> returnValue = ResponsePool.Acquire();

            returnValue.Message = responseMessage;
            returnValue.Uid = incomingUid;
            return returnValue;
        }

        #endregion OODSS methods

        #region WebSocket Handler
        void webSocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (!_firstMessageSent)
            {
                _firstMessageSent = true;
                return;
            }
            CurrentMessage = e.Message;
            Console.WriteLine("Got the message: " + CurrentMessage);

            _lastElementState = (ElementState) _translationScope.Deserialize(CurrentMessage, StringFormat.Json);

            var lastUpdateMessage = _lastElementState as UpdateMessage;
            if (lastUpdateMessage != null)
            {
                Console.WriteLine("This is an update message...");

                ProcessUpdate(lastUpdateMessage);
            }

            _messageReceiveEvent.Set();
        }

        void webSocketClient_DataReceived(object sender, DataReceivedEventArgs e)
        {
            CurrentData = e.Data;
            _dataReceiveEvent.Set();
        }

        void webSocketClient_Closed(object sender, EventArgs e)
        {
            // TODO: handle client close event;
            _closeEevnt.Set();
        }

        void webSocketClient_Opened(object sender, EventArgs e)
        {
            _openedEvent.Set();
        }

        #endregion WebSocket Handler
    }

}
