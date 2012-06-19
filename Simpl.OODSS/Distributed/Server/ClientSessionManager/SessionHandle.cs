using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Simpl.OODSS.Messages;
using ecologylab.collections;

namespace Simpl.OODSS.Distributed.Server.ClientSessionManager
{
    public class SessionHandle
    {
        private BaseSessionManager<Scope<Object>,Scope<Object>> SessionManager { get; set; }

        public IPEndPoint GetSocketAddress()
        {
            return SessionManager.GetAddress();
        }

        public IPAddress GetIpAddress()
        {
            return GetSocketAddress().Address;
        }

        public int GetPortNumber()
        {
            return GetSocketAddress().Port;
        }

        public void SendUpdate<TScope>(UpdateMessage<TScope> update) where TScope : Scope<object>
        {
            SessionManager.SendUpdateToClient(update);
        }

        public Scope<Object> GetSessionScope()
        {
            return SessionManager.GetScope();
        }

        public void Invalidate()
        {
            SessionManager.SetInvalidating(true);
        }

        public Object GetSessionId()
        {
            return SessionManager.GetSessionId();
        }

        public override bool Equals(Object other)
        {
            if (other is SessionHandle)
            {
                return GetSessionId().Equals(((SessionHandle) other).GetSessionId());
            }
            else
            {
                return false;
            }
        }
    }
}
