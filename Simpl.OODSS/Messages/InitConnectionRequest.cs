using System;
using Simpl.OODSS.Messages;
using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
    /// <summary>
    ///  Request to start a new connection to a server. If the message has no sessionId value, then it is
    /// attempting to open a completely new connection. If it has a value for sessionId, it is the
    /// sessionId provided by a previous connection.
    /// 
    /// Sending a message with a past sessionId is no guarantee of restoring the old connection; the
    /// server may have disposed of it.
    /// </summary>
    /// <typeparam name="S"></typeparam>
	[SimplInherit]
	public class InitConnectionRequest<S> : RequestMessage<S> where S : Scope<object>
	{
		[SimplScalar]
		protected String sessionId;

		public InitConnectionRequest()
		{ }

        public InitConnectionRequest(String sessionId)
        {
            this.sessionId = sessionId;
        }

	    public String SessionId
		{
			get{return sessionId;}
			set{sessionId = value;}
		}

        /// <summary>
        /// Returns null. Logic for handling initializing messages must be handled by a ClientSessionScope
	    /// object.
        /// </summary>
        /// <param name="clientSessionScope"></param>
        /// <returns></returns>
	    public override ResponseMessage<S> PerformService(S clientSessionScope)
        {
            return null;
        }
	}
}
