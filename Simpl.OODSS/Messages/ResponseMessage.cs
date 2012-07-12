using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
	/// <summary>
    /// Abstract base class for ecologylab.oodss DCF response messages.
	/// </summary>
	[SimplInherit]
	public abstract class ResponseMessage : ServiceMessage
	{
		public ResponseMessage()
		{
		}

        /// <summary>
        /// Let's the client easily test for OK or error.
        /// </summary>
        /// <returns>true if the response is not an error of some kind.</returns>
	    public abstract bool IsOK();

        /// <summary>
        /// Allows for custom processing of ResponseMessages by ServicesClient,
        /// without extending that.
        /// </summary>
        /// <param name="objectRegistry">
        /// provide a context for response message processing.</param>
	    public virtual void ProcessResponse(Scope<object> objectRegistry)
	    {
	    }
	}
}
