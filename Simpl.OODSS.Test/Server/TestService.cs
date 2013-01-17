using Ecologylab.Collections;
using Simpl.OODSS.Distributed.Server;
using Simpl.OODSS.Messages;
using Simpl.OODSS.Test.TypesScope;
using Simpl.Serialization;

namespace Simpl.OODSS.Test.Server
{
    class TestService
    {
        private Scope<object> _applicationObjectScope;
        private SimplTypesScope _serviceTypesScope;
        private WebSocketOODSSServer _webSocketOODSSServer;

        public TestService(int port)
        {
            _applicationObjectScope = new Scope<object>();
            _serviceTypesScope = TestTypesScope.Get(); 
            _webSocketOODSSServer = new WebSocketOODSSServer(_serviceTypesScope, _applicationObjectScope, -1, -1, port);
        }

        public bool StartService()
        {
            return _webSocketOODSSServer.Start();
        }

        public void StopService()
        {
            _webSocketOODSSServer.Stop();
        }

        

    }
}
