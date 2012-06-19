using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server.ClientSessionManager
{
    public abstract class TCPClientSessionManager<S,P>:BaseSessionManager<S,P>
        where S : Scope<object>
        where P : Scope<object> 
    {
        public TCPClientSessionManager(string sessionId, ServerProcessor frontend, P baseScope) : base(sessionId, frontend, baseScope)
        {
        }
    }
}
