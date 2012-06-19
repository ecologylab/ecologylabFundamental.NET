using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Messages
{
    /// <summary>
    /// Interface to indicate that a message can be sent over the network. Used for type checking in
    /// client send methods, to allow other, more specific interfaces to be used (such as
    /// AuthenticationRequest).
    /// </summary>
    interface ISendableRequest
    {
        bool IsDisposable();
    }
}
