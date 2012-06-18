using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Distributed.Common
{
    static class BaseStates
    {
        /// <summary>
        /// Client not currently connected to any server.
        /// </summary>
        const string NotConnected = "Not connected.";

        /// <summary>
        /// Client currently attempting to connect to a server.
        /// </summary>
        const string Connecting = "Connecting.";

        /// <summary>
        /// Client connected to a server.
        /// </summary>
        const string Connected = "Connected.";
    }
}
