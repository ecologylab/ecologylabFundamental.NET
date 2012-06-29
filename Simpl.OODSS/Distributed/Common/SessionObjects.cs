using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization;
using ecologylab.serialization;

namespace Simpl.OODSS.Distributed.Common
{
    /// <summary>
    /// Constants that define general ecologylab objects that get stored in the
    /// Session ObjectRegistry.
    /// </summary>
    public static class SessionObjects
    {
        public static readonly string MainStartAndStoppable = "main_start_and_stoppable";

        public static readonly string MainShutdownable = "main_shutdownable";

        //public static readonly SimplTypesScope BrowserServicesTranslations = OODSSMessages.Get(); //TODO: is this correct? 

        public static readonly string Logging = "logging";

        public static readonly string TopLevel = "top_level";

        public static readonly string NamedStylesMap = "named_styles_map";

        public static readonly string InterestModelSource = "interest_model_source";

        public static readonly string GraphicsConiguration = "greaphics_configuration";

        public static readonly string SessionsMap = "sessions_map";

        public static readonly string SessionHandle = "SESSION_HANDLE";

        public static readonly string ApplicationEnvironment = "application_environment";

        public static readonly string SessionId = "session_id";

        public static readonly string ClientManager = "CLIENT_MANAGER";

        public static readonly string SessionsMapBySessionId = "sessions_map_by_session_id";

        public static readonly string WebSocketOODSSServer = "oodss_websocket_server";
    }
}
