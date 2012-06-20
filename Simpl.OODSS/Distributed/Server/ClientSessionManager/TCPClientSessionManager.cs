using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.OODSS.Distributed.Common;
using Simpl.OODSS.Distributed.Impl;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server.ClientSessionManager
{
    /// <summary>
    /// The base class for all ContextManagers, objects that track the state and respond to clients on a
    /// server. There is a one-to-one correspondence between connected clients and ContextManager
    /// instances.
    /// 
    /// AbstractContextManager handles all encoding and decoding of messages, as well as translating
    /// them. Hook methods provide places where subclasses may modify behavior for specific purposes.
    /// 
    /// Typical usage is to have the context manager's request queue be filled by a network thread, while
    /// it is emptied by a working thread.
    /// 
    /// The normal cycle for filling the queue is to call acquireIncomingSequenceBuf() to clear and get
    /// the incomingCharBuffer, then fill it externally (normally passing it as an argument to a
    /// CharsetDecoder.decode call), then calling processIncomingSequenceBufToQueue() to release it and
    /// let the ContextManager store the characters, converting messages into objects as they become
    /// available.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="P"></typeparam>
    public abstract class TCPClientSessionManager<S, P> : BaseSessionManager<S, P>
        where S : Scope<object>
        where P : Scope<object>
    {
        #region Datamember 

        /// <summary>
        /// Stores the key-value pairings from a parsed HTTP-like header on an incoming message.
        /// </summary>
        protected readonly Dictionary<string, string> headerMap = new Dictionary<string, string>();

        protected int startReadIndex = 0;

        /// <summary>
        /// Stores outgoing header character data.
        /// </summary>
        protected readonly StringBuilder HeaderBufOutgoing = new StringBuilder(NetworkConstants.MaxHttpHeaderLength);

        protected readonly StringBuilder StartLine = new StringBuilder(NetworkConstants.MaxHttpHeaderLength);

        // TODO: java code
        //protected NIOServerIOThread

        /// <summary>
        /// The maximum message length allowed for clients that connect to this session manager. Note that
        /// most of the buffers used by AbstractClientManager are mutable in size, and will dynamically
        /// reallocate as necessary if they were initialized to be too small.
        /// </summary>
        protected int MaxMessageSize;

        /// <summary>
        /// Used to translate incoming message XML strings into RequestMessages.
        /// </summary>
        protected SimplTypesScope TranslationScope;

        /// <summary>
        /// stores the sequence of characters read from the header of an incoming message, may need to
        /// persist across read calls, as the entire header may not be sent at once.
        /// </summary>
        private readonly StringBuilder currentHeaderSequence = new StringBuilder();

        /// <summary>
        /// stores the sequence of characters read from the header of an incoming message and identified as
        /// being a key for a header entry; may need to persist across read calls.
        /// </summary>
        private readonly StringBuilder currentKeyHeaderSequence = new StringBuilder();

        /// <summary>
        /// Tracks the number of bad transmissions from the client; used for determining if a client is
        /// bad.
        /// </summary>
        private int badTransmissionCount;

        private int endOfFirstHeader = -1;

        /// <summary>
        /// Counts how many characters still need to be extracted from the incomingMessageBuffer before
        /// they can be turned into a message (based upon the HTTP header). A value of -1 means that there
        /// is not yet a complete header, so no length has been determined (yet).
        /// </summary>
        private int contentLengthRemaining = -1;

        /// <summary>
        /// Specifies whether or not the current message uses compression.
        /// </summary>
        private string contentEncoding = "identity";

        /// <summary>
        /// Set of encoding schemes that the client supports
        /// </summary>
        private HashSet<string> availableEncoding = new HashSet<string>();

        /// <summary>
        /// Stores the first XML message from the incomingMessageBuffer, or parts of it (if it is being
        /// read over several invocations).
        /// </summary>
        private StringBuilder persistentMessageBuffer;

        private long contentUid = -1;

        // TODO: zip 
        // java code:
        // private Inflater
        // private Deflater

        // TODO: message with metadata queue and pool
        //	protected final Queue<MessageWithMetadata<RequestMessage, Object>>	requestQueue							= new LinkedBlockingQueue<MessageWithMetadata<RequestMessage, Object>>();
        /// protected final MessageWithMetadataPool<RequestMessage, Object>			reqPool										= new MessageWithMetadataPool<RequestMessage, Object>(

        protected Encoding decoder = NetworkConstants.Charset;

        protected Encoding encoder = NetworkConstants.Charset;

        private static readonly String PostPrefix = "POST ";

        private static readonly String GetPrefix = "GET ";

        #endregion Datamember

        #region Constructer

        /// <summary>
        ///
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="frontend"></param>
        /// <param name="baseScope"></param>
        public TCPClientSessionManager(string sessionId, int maxMessageSize, ServerProcessor frontend,
                                       SimplTypesScope translationScope, P baseScope)
            : base(sessionId, frontend, baseScope)
        {
            SessionId = sessionId;
            TranslationScope = translationScope;
            MaxMessageSize = maxMessageSize;

            Handle = new SessionHandle(this);
            LocalScope.Add(SessionObjects.SessionHandle, Handle);

            PrepareBuffers(HeaderBufOutgoing);
        }

        #endregion Constructer

        #region Methods

        // TODO: java code 
        // public synchronized final void processIncomingSequenceBufToQueue(CharBuffer incomingSequenceBuf) throws CharacterCodingException, BadClientException

        // TODO: java code 
        // 	private CharSequence unCompress(StringBuilder firstMessageBuffer) throws CharacterCodingException, DataFormatException

        //sealed? final
        public void ProcessAllMessagesAndSendResponses<T>()
        {
            while (IsMessageWaiting())
            {
                this.ProcessNextMessageAndSendResponse<T>();
            }
        }

        // TODO: java code
        // public void setSocket(SelectionKey socket)

        protected abstract void ClearOutgoingMessageBuffer(StringBuilder outgoingMessageBuf);

        protected abstract void ClearOutgoingMessageHeaderBuffer(StringBuilder outgoingMessageHeaderBuf);

        protected abstract void CreateHeader<T>(int messageSize, StringBuilder outgoingMessageHeaderBuf,
                                                RequestMessage<T> incomingRequest, ResponseMessage<T> outgoingResponse,
                                                long uid);

        protected abstract void MakeUpdateHeader<T>(int messageSize, StringBuilder headerBufOutgoing,
                                                    UpdateMessage<T> update);

        /// <summary>
        /// Parses the header of an incoming set of characters (i.e. a message from a client to a server),
        /// loading all of the HTTP-like headers into the given headerMap.
        /// 
        /// If headerMap is null, this method will throw a null pointer exception.
        /// </summary>
        /// <param name="startChar"></param>
        /// <param name="allIncomingChars"></param>
        /// <returns>the length of the parsed header, or -1 if it was not yet found.</returns>
        protected int ParseHeader(int startChar, StringBuilder allIncomingChars)
        {
            // TODO:
            return -1;
        }

        protected abstract void PrepareBuffers(StringBuilder outgoingMessageHeaderBuf);

        protected abstract void TranslateResponseMessageToStringBufferContents<T>(RequestMessage<T> requestMessage,
                                                                                  ResponseMessage<T> responseMessage,
                                                                                  StringBuilder messageBuffer);

        /// <summary>
        /// Translates the given XML String into a RequestMessage object.
        ///
        /// translateStringToRequestMessage(String) may be overridden to provide specific functionality,
        /// such as a ContextManager that does not use XML Strings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageCharSequence">an XML String representing a RequestMessage object.</param>
        /// <returns>the RequestMessage created by translating messageString into an object.</returns>
        protected RequestMessage<T> TranslateStringToRequestMessage<T>(string messageCharSequence)
        {
            string startLineString = null;
            if ((StartLine == null) || (startLineString = StartLine.ToString()).Equals(""))
            {
                return TranslateOODSSRequest<T>(messageCharSequence, startLineString);
            }
            else if (startLineString.StartsWith(GetPrefix))
            {
                return TranslateGetRequest<T>(messageCharSequence, startLineString);
            }
            else if (startLineString.StartsWith(PostPrefix))
            {
                return TranslatePostRequest<T>(messageCharSequence, startLineString);
            }
            else
            {
                return TranslateOtherRequest<T>(messageCharSequence, startLineString);
            }
        }

        /// <summary>
        /// Translates an incoming character sequence identified to be an OODSS request message (not a GET
        /// or POST request).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageCharSequence"></param>
        /// <param name="startLineString"></param>
        /// <returns>The request message contained in the message.</returns>
        protected RequestMessage<T> TranslateOODSSRequest<T>(string messageCharSequence, string startLineString)
        {
            return (RequestMessage<T>) TranslationScope.Deserialize(messageCharSequence, StringFormat.Xml);
        }

        /// <summary>
        /// Translates an incoming character sequence identified to be a GET request.
        /// This implementation returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageCharSequence"></param>
        /// <param name="startLineString"></param>
        /// <returns></returns>
        protected RequestMessage<T> TranslateGetRequest<T>(string messageCharSequence, string startLineString)
        {
            return null;
        }

        /// <summary>
        /// Translates an incoming character sequence identified to be a POST request.
        /// This implementation expects the POST request to contain a nested OODSS request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageCharSequence"></param>
        /// <param name="startLineString"></param>
        /// <returns></returns>
        protected RequestMessage<T> TranslatePostRequest<T>(string messageCharSequence, string startLineString)
        {
            string messageString = messageCharSequence;
            if (!messageString.StartsWith("<"))
                messageString = messageString.Substring(messageString.IndexOf("=") + 1);

            return TranslateOODSSRequest<T>(messageString, startLineString);
        }

        /// <summary>
        /// Translates an incoming character sequence that cannot be identified. Called when the first line
        /// of the request is not empty, not GET, and not POST.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageCharSequence"></param>
        /// <param name="startLineString"></param>
        /// <returns>null</returns>
        protected RequestMessage<T> TranslateOtherRequest<T>(string messageCharSequence, string startLineString)
        {
            return null;
        }

        /// <summary>
        /// Adds the given request to this's request queue.
        ///
        /// enqueueRequest(RequestMessage) is a hook method for ContextManagers that need to implement
        /// other functionality, such as prioritizing messages.
        /// 
        /// If enqueueRequest(RequestMessage) is overridden, the following methods should also be
        /// overridden: isMessageWaiting(), getNextRequest().
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="request"></param>
        protected void EnqueueRequest<M>(MessageWithMetadata<M, Object> request)
        {
            //TODO: messageWaiting = requestQueue.Offer(request)
        }

        /// <summary>
        /// Returns the next message in the request queue.
        ///
        /// getNextRequest() may be overridden to provide specific functionality, such as a priority queue.
        /// In this case, it is important to override the following methods: isMessageWaiting(),
        /// enqueueRequest().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected MessageWithMetadata<RequestMessage<T>, object> GetNextRequest<T>()
        {
            //TODO:
        }

        /// <summary>
        /// Calls processRequest(RequestMessage) on the result of getNextRequest().
        /// 
        /// In order to override functionality processRequest(RequestMessage) and/or getNextRequest()
        /// should be overridden.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void ProcessNextMessageAndSendResponse<T>()
        {
            ProcessRequest<T>(GetNextRequest<T>());
        }
        
        /// <summary>
        /// Calls performService(requestMessage), then converts the resulting ResponseMessage into a
	    /// String, adds the HTTP-like headers, and passes the completed String to the server backend for
	    /// sending to the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestWithMetadata"></param>
        /// <returns>the request message to process.</returns>
        protected ResponseMessage<T> ProcessRequest<T>(
            MessageWithMetadata<RequestMessage<T>, object> requestWithMetadata)
        {
            RequestMessage<T> reqeust = requestWithMetadata.GetMessage();
            ResponseMessage<T> response = base.ProcessRequest(request, getIPAddresss());

            if (response != null)
            {
                SendResponseToClient(requestWithMetadata, response, request);
            }
            else
            {
                Console.WriteLine("context manager did not produce a response message.");
            }
            requestWithMetadata = reqPool.release(requestWithMetadata);

            return response;
        }

        private void SendResponseToClient<T>(MessageWithMetadata<T, object> requestWithMetadata,
                                          ResponseMessage<T> response, RequestMessage<T> request) where T: Scope<object>
        {
            //TODO:
        }

        public override void SendUpdateToClient<TScope>(UpdateMessage<TScope> update)
        {
        }

        
        private void Compress(StringBuilder src, byte[] dest)
        {
            //TODO: compress
        }

        /// <summary>
        /// Takes an incoming message in the form of an XML String and converts it into a RequestMessage
	    /// using translateStringToRequestMessage(String). Then places the RequestMessage on the
	    /// requestQueue using enqueueRequest().
        /// </summary>
        /// <param name="incomingMessage"></param>
        /// <param name="incomingUid"></param>
        private void ProcessString(string incomingMessage, long incomingUid)
        {
            // TODO:
        }

        public override IPEndPoint GetAddress()
        {
            return null;
        }

        public SessionHandle GetHandle()
        {
            return Handle;
        }

        #endregion Methods

    }
}
