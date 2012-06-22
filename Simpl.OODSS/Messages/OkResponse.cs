using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
	/// <summary>
    /// Base class for all ResponseMessages that were processed successfully.
	/// </summary>
	[SimplInherit]
	public class OkResponse: ResponseMessage
	{
        public static readonly OkResponse ReusableInstance = new OkResponse(); 

		public OkResponse()
		{ }

	    public static OkResponse Get()
	    {
	        return ReusableInstance;
	    }

	    public override bool IsOK()
	    {
	        return true;
	    }
	}
}
