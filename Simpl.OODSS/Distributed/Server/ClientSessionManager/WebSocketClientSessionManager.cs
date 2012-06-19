using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.OODSS.Messages;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server.ClientSessionManager
{
    class WebSocketClientSessionManager<S,P> : TCPClientSessionManager<S,P> 
        where S : Scope<object> 
        where P : Scope<object>
    {
        public WebSocketClientSessionManager(string sessionId, ServerProcessor frontend, P baseScope) : base(sessionId, frontend, baseScope)
        {
        }

        public override IPEndPoint GetAddress()
        {
            throw new NotImplementedException();
        }

        public override void SendUpdateToClient<T>(UpdateMessage<T> update)
        {
            throw new NotImplementedException();
        }
    }
}
