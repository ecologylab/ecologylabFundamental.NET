using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.OODSS.Messages;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server.ClientSessionManager
{
    public abstract class BaseSessionManager<S, T> where S:Scope<T>
    {
        /// <summary>
        /// Indicates whether or not one or more messages are queued for execution by this ContextManager.
        /// </summary>
        protected bool MessageWaiting { get; set; }

        /// <summary>
        /// Session handle available to use by clients
        /// </summary>
        protected SessionHandle Handle;

        protected string SessionId = null;

        protected S LocalScope;

        protected long LastActivity = DateTime.Now.Ticks;

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

        public BaseSessionManager<S, T>

        public IPEndPoint GetAddress()
        {
            throw new NotImplementedException();
        }

        internal void SendUpdateToClient<T>(UpdateMessage<T> update)
        {
            throw new NotImplementedException();
        }

        internal Scope<T> GetScope<T>()
        {
            throw new NotImplementedException();
        }

        internal void SetInvalidating(bool p)
        {
            throw new NotImplementedException();
        }

        internal object GetSessionId()
        {
            throw new NotImplementedException();
        }
    } 
}
