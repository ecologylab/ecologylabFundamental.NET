using System.Threading.Tasks;
using Ecologylab.Collections;
using Simpl.OODSS.Distributed.Client;
using Simpl.OODSS.Messages;
using Simpl.OODSS.TestClientAndMessage.Messages;
using Simpl.OODSS.TestClientAndMessage.TypesScope;
using Simpl.Serialization;

namespace Simpl.OODSS.TestClientAndMessage.Client
{
    public class TestServiceClient : ITestServiceUpdateListener
    {
        private readonly string _serviceAddress;
        private readonly int _port;

        private WebSocketOODSSClient _client;
        private readonly SimplTypesScope _testTypesScope;
        private readonly Scope<object> _clientScope;

        public TestServiceClient(string serviceAddress, int port)
        {
            _serviceAddress = serviceAddress;
            _port = port;
            _testTypesScope = TestClientTypesScope.Get();
            _clientScope = new Scope<object>();
            _clientScope.Add(TestServiceConstants.ServiceUpdateListener, this);
            _client = new WebSocketOODSSClient(_serviceAddress, _port, _testTypesScope, _clientScope);
        }

        public void OnReceiveUpdate(TestServiceUpdate response)
        {
            //throw new NotImplementedException();
        }

        public async Task<ResponseMessage> SendMessage(string message)
        {
            var request = new TestServiceRequest(message);
            return await _client.RequestAsync(request);
        }

        public async Task<bool> StartConnection()
        {
            return await _client.StartAsync();
        }

        public void StopClient()
        {
            _client.StopClient();
        }
    }

}
