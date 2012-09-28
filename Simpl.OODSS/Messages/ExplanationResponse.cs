using System;
using Simpl.Serialization.Attributes;
using Ecologylab.Collections;

namespace Simpl.OODSS.Messages 
{
	/// <summary>
    /// Base class for all ResponseMessages that indicate errors.
	/// </summary>
	[SimplInherit]
	public class ExplanationResponse : ResponseMessage
	{
		/// <summary>
		/// missing java doc comments or could not find the source file.
		/// </summary>
		[SimplScalar]
		protected String explanation;

		public ExplanationResponse()
		{ }

        public ExplanationResponse(string explanation)
            :this()
        {
            this.explanation = explanation;
        }

	    public String Explanation
		{
			get{return explanation;}
			set{explanation = value;}
		}

	    public override bool IsOK()
	    {
	        return true;
	    }
	}
}
