using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.OODSS.Logging;
using Simpl.Serialization;

namespace Simpl.OODSS.Messages
{
    public class DefaultServicesTranslations
    {
        public const string PackageName = "ecologylab.oodss.messages";

        public static readonly Type[] Translations =
            {
                typeof(RequestMessage),
                typeof(ResponseMessage),
                typeof(CloseMessage),
                typeof(OkResponse),
                typeof(BadSemanticContentResponse),
                typeof(ErrorResponse),
                typeof(Prologue),
                typeof(Epilogue),
                typeof(LogOps),
                typeof(SendEpilogue),
                typeof(SendPrologue),
                typeof(HttpRequest),
                typeof(PingRequest),
                typeof(Ping),
                typeof(Pong),
                typeof(UrlMessage),
                typeof(CfCollaborationGetSurrogate),
                typeof(ContinuedHTTPGetRequest),
                typeof(IgnoreRequest),
                typeof(InitConnectionRequest),
                typeof(InitConnectionResponse),
                typeof(DisconnectRequest),
                typeof(ServiceMessage),
                typeof(UpdateMessage)
            };

        public static SimplTypesScope Get()
        {
            return SimplTypesScope.Get(PackageName, Translations);
        }
    }
}
