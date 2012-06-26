using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.OODSS.Messages;
using Simpl.Serialization;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server.ClientSessionManager
{
    public abstract class BaseSessionManager
    {
        /// <summary>
        /// Indicates whether or not one or more messages are queued for execution by this ContextManager.
        /// </summary>
        protected bool MessageWaiting { get; set; }

        /// <summary>
        /// Session handle available to use by clients
        /// </summary>
        protected SessionHandle Handle;

        public string SessionId { 
            get;
            private set;
        }


        protected Scope<object> LocalScope;

        protected long LastActivity = DateTime.Now.Ticks;

        //TODO: selectionKey
        // java: protected SelectionKey				socketKey;

        /// <summary>
        /// The frontend for the server that is running the ContextManager. This is needed in case the
        /// client attempts to restore a session, in which case the frontend must be queried for the old
        /// ContextManager.
        /// </summary>
        protected ServerProcessor FrontEnd = null;

        /// <summary>
        /// Indicates whether the first request message has been received. The first request may be an
	    /// InitConnection, which has special properties.
        /// </summary>
        protected bool Initialized = false;

        /// <summary>
        /// Used for disconnecting. A disconnect message will call the setInvalidating method, which will
	    /// set this value to true. The processing method will set itself as pending invalidation after it
	    /// has produces the bytes for the response to the disconnect message.
        /// </summary>
        private bool _invalidating = false;

        public static readonly string SESSION_ID = "SESSION_ID";

        public static readonly string CLIENT_MANAGER = "CLIENT_MANAGER";

        protected SimplTypesScope TranslationScope;

        public BaseSessionManager(string sessionId, SimplTypesScope translationScope, Scope<object> applicationObjectScope, ServerProcessor frontend)
        {
            FrontEnd = frontend;
            SessionId = sessionId;
            LocalScope = applicationObjectScope;
            TranslationScope = translationScope;
            LocalScope.Add(SESSION_ID, sessionId);
            LocalScope.Add(CLIENT_MANAGER, this);
        }

        /// <summary>
        /// Provides the context scope for the client attached to this session manager. The base
	    /// implementation instantiates a new Scope<?> with baseScope as the argument. Subclasses may
	    /// provide specific subclasses of Scope as the return value. They must still incorporate baseScope
	    /// as the lexically chained application object scope.
        /// </summary>
        /// <param name="baseScope"></param>
        /// <returns></returns>
        private Scope<object> GenerateContextScope(Scope<object> baseScope)
        {
            return new Scope<Object>(baseScope);
        }

        ///// <summary>
        ///// Appends the sender's IP address to the incoming message and calls performService on the given
        ///// RequestMessage using the local ObjectRegistry.
        ///// 
        ///// performService(RequestMessage) may be overridden by subclasses to provide more specialized
        ///// functionality. Generally, overrides should then call super.performService(RequestMessage) so
        ///// that the IP address is appended to the message.
        ///// </summary>
        ///// <param name="requestMessage"></param>
        ///// <param name="ipAddress"></param>
        ///// <returns></returns>
        //protected ResponseMessage PerformService(RequestMessage requestMessage, IPAddress ipAddress)
        //{
        //    requestMessage.Sender = ipAddress;

        //    try
        //    {
        //        return requestMessage.PerformService(LocalScope);
        //    }
        //    catch (Exception e)
        //    {
                
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Calls RequestMessage.performService(Scope) and returns the result.
        ///// </summary>
        ///// <param name="request">- the request message to process.</param>
        ///// <param name="ipAddress"></param>
        ///// <returns></returns>
        //protected ResponseMessage ProcessRequest(RequestMessage request, IPAddress ipAddress)
        //{
        //    LastActivity = DateTime.Now.Ticks;
        //    ResponseMessage response = null;

        //    if (request == null)
        //    {
        //        Console.WriteLine("No request.");
        //    }
        //    else
        //    {
        //        if (!Initialized)
        //        {
        //            // special processing for InitConnectionRequest
        //            if (request is InitConnectionRequest)
        //            {
        //                string incomingSessionId = ((InitConnectionRequest) request).SessionId;

        //                if (incomingSessionId == null)
        //                {
        //                    // client is not expecting an old ContextManager
        //                    response = new InitConnectionResponse(SessionId);
        //                }
        //                else
        //                {
        //                    // client is expecting an old ContextManager
        //                    if (FrontEnd.RestoreContextManagerFromSessionId(incomingSessionId, this))
        //                    {
        //                        response = new InitConnectionResponse(incomingSessionId);
        //                    }
        //                    else
        //                    {
        //                        response = new InitConnectionResponse(SessionId);
        //                    }
        //                }

        //                Initialized = true;
        //            }
        //        }
        //        else
        //        {
        //            response = PerformService(request, ipAddress);
        //        }

        //        if (response == null)
        //        {
        //            Console.WriteLine("Context manager did not produce a response message.");
        //        }
        //    }

        //    return response;
        //}

        /// <summary>
        /// Indicates the last System timestamp was when the ContextManager had any activity.
        /// </summary>
        /// <returns></returns>
        public long GetLastActivity()
        {
            return LastActivity;
        }

        /// <summary>
        /// Indicates whether there are any messages queued up to be processed.
        ///
        /// isMessageWaiting() should be overridden if getNextRequest() is overridden so that it properly
        /// reflects the way that getNextRequest() works; it may also be important to override
        /// enqueueRequest().
        /// </summary>
        /// <returns>true if getNextRequest() can return a value, false if it cannot.</returns>
        public bool IsMessageWaiting()
        {
            return MessageWaiting;
        }

        /// <summary>
        /// Indicates whether or not this context manager has been initialized. Normally, this means that
        /// it has shared a session id with the client.
        /// </summary>
        /// <returns></returns>
        public bool IsInitialized()
        {
            return Initialized;
        }

        public abstract IPEndPoint GetAddress();

        public abstract void SendUpdateToClient(UpdateMessage update, string receivingSessionId);

        public Scope<Object> GetScope()
        {
            return LocalScope;
        }

        public void SetInvalidating(bool invalidating)
        {
            _invalidating = invalidating;
        }

        public Boolean IsInvalidating()
        {
            return _invalidating;
        }

        public String GetSessionId()
        {
            return SessionId;
        }

        /// <summary>
        /// Hook method for having shutdown behavior.
        /// This method is called whenever the server is closing down the connection to this client.
        /// </summary>
        public void Shutdown()
        {
        }
    } 
}
