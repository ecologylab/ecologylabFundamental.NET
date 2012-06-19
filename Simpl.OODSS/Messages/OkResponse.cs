using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
	/// <summary>
    /// Base class for all ResponseMessages that were processed successfully.
	/// </summary>
	[SimplInherit]
	public class OkResponse<S> : ResponseMessage<S> where S : Scope<object>
	{
        public static readonly OkResponse<S> reusableInstance = new OkResponse<S>(); 

		public OkResponse()
		{ }

	    public static OkResponse<S> Get()
	    {
	        return reusableInstance;
	    }

	    public override bool IsOK()
	    {
	        return true;
	    }
	}
}
