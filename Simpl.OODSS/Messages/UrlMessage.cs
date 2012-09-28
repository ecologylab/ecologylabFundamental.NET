using System;
using Simpl.Serialization.Attributes;
using Ecologylab.Collections;

namespace Simpl.OODSS.Messages 
{
	[SimplInherit]
    public class UrlMessage : RequestMessage
	{
		[SimplScalar]
		protected String url;

		[SimplScalar]
		protected String collection;

		public UrlMessage()
		{ }

		public String Url
		{
			get{return url;}
			set{url = value;}
		}

		public String Collection
		{
			get{return collection;}
			set{collection = value;}
		}

	    public override ResponseMessage PerformService(Scope<object> clientSessionScope)
	    {
	        throw new NotImplementedException();
	    }
	}
}
