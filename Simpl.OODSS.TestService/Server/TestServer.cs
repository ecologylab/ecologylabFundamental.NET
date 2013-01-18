using Ecologylab.Collections;
using Simpl.OODSS.Distributed.Server;
using Simpl.OODSS.TestClientAndMessage.TypesScope;
using Simpl.OODSS.TestService.TypesScope;
using Simpl.Serialization;

namespace Simpl.OODSS.TestService.Server
{
    public class TestServer
    {
        private Scope<object> _applicationObjectScope;
        private SimplTypesScope _serviceTypesScope;
        private WebSocketOODSSServer _webSocketOODSSServer;

        public TestServer(int port)
        {
            _applicationObjectScope = new Scope<object>();
            _serviceTypesScope = TestServiceTypesScope.Get(); 
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
