using Simpl.Fundamental.Net;
using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
	[SimplInherit]
	public abstract class RequestMessage : ServiceMessage, ISendableRequest
	{
		public RequestMessage()
		{
		}

        /// <summary>
        /// Perform the service associated with the request, using the supplied context as needed.
        /// </summary>
        /// <param name="clientSessionScope">Context to perform it in/with.</param>
        /// <returns>Response to pass back to the (remote) caller.</returns>
	    public abstract ResponseMessage PerformService(Scope<object> clientSessionScope);

        /// <summary>
        /// Indicates whether or not this type of message may be ignored by the server, if the server
	    /// becomes backed-up. For example, a RequestMessage subclass that simply requests the server's
	    /// current state may be ignored if a more recent copy of one has arrived later.
	    /// 
	    /// By default, RequestMessages are not disposable; this method should be overriden if they are to
	    /// be.
        /// </summary>
        /// <returns>false</returns>
	    public bool IsDisposable()
	    {
	        return false;
	    }

        /// <summary>
        /// A URL can be provided, indicating the response should be accomplished with HTTP redirect. Used
	    /// when browser security is an issue.
	    /// 
        /// This is the redirect URL for response when processing is successful.
        /// </summary>
        /// <param name="clientSessionScope">
        /// Can be used to generate HTTP GET style arguments in the redirect URL.</param>
        /// <returns>null in this the base class case.</returns>
        public ParsedUri OkRedirectUri(Scope<object> clientSessionScope)
        {
            return null;
        }

        /// <summary>
        /// A URL can be provided, indicating the response should be accomplished with HTTP redirect. Used
        /// when browser security is an issue.
        /// 
        /// This is the redirect URL for response when processing results in an error.
        /// </summary>
        /// <param name="clientSessionScope">
        /// Can be used to generate HTTP GET style arguments in the redirect URL.</param>
        /// <returns>null in this the base class case.</returns>
        public ParsedUri ErrorRedirectUri(Scope<object> clientSessionScope)
        {
            return null;
        }

	}
}
