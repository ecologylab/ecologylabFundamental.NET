using System;
using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
	/// <summary>
	/// Response to a request to connect to a server. On a successful connection, sessionId will contain
    /// the server-assigned session identifier. If the connection failed, sessionId will be null.
	/// </summary>
	[SimplInherit]
	public class InitConnectionResponse<S> : ResponseMessage<S> where S : Scope<object>
	{
        /// <summary>
        /// The session identifier used for all communications between this client and the server. If the
	    /// value is null, it means the connection failed.
        /// </summary>
		[SimplScalar]
		protected String sessionId;

		public InitConnectionResponse()
		{ }

        public InitConnectionResponse(string sessionId)
        {
            this.sessionId = sessionId;
        }

		public String SessionId
		{
			get{return sessionId;}
			set{sessionId = value;}
		}

	    public override bool IsOK()
	    {
	        return sessionId != null;
	    }
	}
}
