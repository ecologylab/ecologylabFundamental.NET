using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Distributed.Common
{
    static class ClientConstants
    {
        /// <summary>
        /// Number of reconnect attempts to make before giving up.
        /// </summary>
        static readonly int ReconnectAttempts = 50;

        /// <summary>
        /// Number of milliseconds to sleep between attempts to reconnect.
        /// </summary>
        static readonly int WaitBetweenReconnectAttempts = 3000;
    }
}
