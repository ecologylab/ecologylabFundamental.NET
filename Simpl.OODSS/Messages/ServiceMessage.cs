using System;
using System.Net;
using Simpl.Serialization;
using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
	/// <summary>
    /// Abstract base class for ecologylab.oodss DCF request and response messages.
	/// </summary>
	public class ServiceMessage<S> : ElementState where S:Scope<Object>
	{
        public ServiceMessage()
        { }

		[SimplScalar]
		protected long timeStamp;

        public void stampTime()
        {
            timeStamp = DateTime.Now.Ticks;
        }

	    public long TimeStamp
		{
			get { return timeStamp; }
			set { timeStamp = value; }
		}

        /// <summary>
        /// Contains the IP address of the host that sent this message. sender currently must be set by a
	    /// server that receives the message and associates it with the IP address from it's packet and/or
	    /// channel.
        /// </summary>
	    protected IPAddress sender;

        /// <summary>
        /// This method should be called by a server when it translates this message.
        /// </summary>
	    public IPAddress Sender
	    {
            get { return sender; }
            set { sender = value; }
	    }
	}
}
