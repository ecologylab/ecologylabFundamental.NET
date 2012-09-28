using System.Collections.Generic;
using Simpl.Serialization.Attributes;
using Ecologylab.Collections;

namespace Simpl.OODSS.Messages 
{
	/// <summary>
    /// Base class for all ResponseMessages that indicate errors.
	/// </summary>
	[SimplInherit]
    public class ErrorResponse : ExplanationResponse
	{
		public ErrorResponse()
		{ }

        public ErrorResponse(string response)
            :this()
        {
            explanation = response;
        }

        public bool IsOK()
        {
            return false;
        }

	}
}
