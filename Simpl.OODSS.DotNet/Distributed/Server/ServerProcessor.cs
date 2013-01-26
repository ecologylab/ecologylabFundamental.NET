using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.OODSS.Distributed.Server.ClientSessionManager;

namespace Simpl.OODSS.Distributed.Server
{
    public interface ServerProcessor
    {
        bool RestoreContextManagerFromSessionId(string incomingSessionId, string newSessionId, BaseSessionManager baseSessionManager);
    }
}
