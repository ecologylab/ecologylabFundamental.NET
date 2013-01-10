using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.OODSS.Distributed.Server
{
    interface IServerMessages
    {
        void PutServerObject(object o);

        string GetAPushFromWebSocket(string s, string sessionId);

        void NewClientAdded(string sessionId);
    }
}
