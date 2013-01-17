using System;
using System.Threading.Tasks;
using Simpl.OODSS.Test.Messages;
using Simpl.OODSS.Distributed.Client;
using Ecologylab.Collections;
using Simpl.OODSS.Test.TypesScope;
using Simpl.Serialization;
using Simpl.OODSS.Messages;

namespace Simpl.OODSS.Test.Client
{
    class TestServiceClient : ITestServiceUpdateListener
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
            _testTypesScope = TestTypesScope.Get();
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
