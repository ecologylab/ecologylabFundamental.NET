using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Distributed.Client
{
    interface ClientStatusListener
    {
        /// <summary>
        /// Invoked when the client's connection status changes.
        /// </summary>
        /// <param name="connect">true if the client connected; 
        ///     false if the client disconnected.</param>
        void ClientConnectionStatusChanged(bool connect);
    }
}
