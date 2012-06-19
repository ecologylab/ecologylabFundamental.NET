using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
	/// <summary>
    /// Abstract base class for asynchronous server-to-client updates.
	/// </summary>
	[SimplInherit]
	public abstract class UpdateMessage<S> : ServiceMessage<S> where S : Scope<object>
	{
		public UpdateMessage()
		{
		}

        /// <summary>
        /// Allows for custom processing of ResponseMessages by ServicesClient,
        /// without extending that.
        /// </summary>
        /// <param name="objectRegistry">provide a context for response message processing.</param>
        public void ProcessUpdate (S objectRegistry)
        {
        }
	}
}
