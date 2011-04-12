using System;
using System.Collections.Generic;
using ecologylab.attributes;
using ecologylab.oodss.messages;
using ecologylab.serialization;
using ecologylab.oodss.logging;

//developer should modify the namespace
//by default it falls into ecologylab.serialization
namespace ecologylab.serialization 
{
	public class OODSSMessages
	{
		public OODSSMessages()
		{ }

		public static TranslationScope Get()
		{
			return TranslationScope.Get("OODSSMessages",
				typeof(Pong),
				typeof(ServiceMessage<object>),
				typeof(InitConnectionRequest),
				typeof(LogEvent),
				typeof(SendEpilogue),
				typeof(OkResponse),
				typeof(CfCollaborationGetSurrogate),
                typeof(ResponseMessage<ServiceMessage<object>>),
				typeof(LogOps),
                typeof(UpdateMessage<ServiceMessage<object>>),
				typeof(Ping),
				typeof(ErrorResponse),
				typeof(SendPrologue),
				typeof(HttpRequest),
				typeof(Prologue),
				typeof(BadSemanticContentResponse),
				typeof(DisconnectRequest),
				typeof(LogueMessage),
                typeof(RequestMessage<ServiceMessage<object>>),
				typeof(Epilogue),
				typeof(InitConnectionResponse),
				typeof(IgnoreRequest),
				typeof(UrlMessage),
				typeof(ContinuedHTTPGetRequest),
                typeof(ExplanationResponse<ServiceMessage<object>>),
				typeof(CloseMessage),
				typeof(HttpGetRequest),
				typeof(PingRequest));
		}
	}
}
