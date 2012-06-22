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
        public const int ReconnectAttempts = 50;

        /// <summary>
        /// Number of milliseconds to sleep between attempts to reconnect.
        /// </summary>
        public const int WaitBetweenReconnectAttempts = 3000;
    }
}
