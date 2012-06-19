using System;
using Simpl.Serialization.Attributes;
using ecologylab.collections;

namespace Simpl.OODSS.Messages 
{
	[SimplInherit]
    public class UrlMessage<S> : RequestMessage<S> where S : Scope<object>
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

	    public override ResponseMessage<S> PerformService(S clientSessionScope)
	    {
	        return OkResponse<S>.Get();
	    }
	}
}
