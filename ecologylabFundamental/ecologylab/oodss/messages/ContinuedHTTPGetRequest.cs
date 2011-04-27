//
//  ContinuedHTTPGetRequest.cs
//  s.im.pl serialization
//
//  Generated by DotNetTranslator on 04/09/11.
//  Copyright 2011 Interface Ecology Lab. 
//

using System;
using System.Collections.Generic;
using ecologylab.attributes;

namespace ecologylab.oodss.messages 
{
	/// <summary>
	/// missing java doc comments or could not find the source file.
	/// </summary>
	[simpl_inherit]
	public class ContinuedHTTPGetRequest : HttpRequest
	{
		/// <summary>
		/// missing java doc comments or could not find the source file.
		/// </summary>
		[simpl_scalar]
		[simpl_hints(new Hint[] { Hint.XML_LEAF_CDATA })]
		private String messageFragment;

		/// <summary>
		/// missing java doc comments or could not find the source file.
		/// </summary>
		[simpl_scalar]
		private Boolean isLast;

		public ContinuedHTTPGetRequest()
		{ }

		public String MessageFragment
		{
			get{return messageFragment;}
			set{messageFragment = value;}
		}

		public Boolean IsLast
		{
			get{return isLast;}
			set{isLast = value;}
		}
	}
}