using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.OODSS.Distributed.Server;
using Simpl.OODSS.Test.Client;
using Simpl.OODSS.Test.Messages;
using Simpl.OODSS.Test.Server;
using Simpl.OODSS.Messages;
using Simpl.OODSS.Test.TypesScope;
using Simpl.Serialization;

namespace Simpl.OODSS.Test
{
    [TestClass]
    public class WebSocketOODSSServiceTest
    {
        private int _port = 2018;
        private string _serverAddress = "localhost";

        private TestService _testService;
        private TestServiceClient _testServiceClient;

        [TestMethod]
        public async Task TestWebSocketOODSSService()
        {
            _testService = new TestService(_port);
            Assert.AreEqual(true, _testService.StartService());

            _testServiceClient = new TestServiceClient(_serverAddress, _port);
            bool clientConnected = await _testServiceClient.StartConnection();
            Assert.AreEqual(true, clientConnected);

            const string testString = "Hello world!";
            var response = (await _testServiceClient.SendMessage(testString)) as TestServiceResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(TestServiceConstants.ServicePrefix + testString, response.Message);

            response = (await _testServiceClient.SendMessage(testString)) as TestServiceResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(TestServiceConstants.ServicePrefix + testString, response.Message);

            _testServiceClient.StopClient();
            // assert client is stopped?
            _testService.StopService();
        }

        [TestMethod]
        public async Task TestLargeSizeMessage()
        {
            _testService = new TestService(_port);
            Assert.AreEqual(true, _testService.StartService());

            _testServiceClient = new TestServiceClient(_serverAddress, _port);
            bool clientConnected = await _testServiceClient.StartConnection();
            Assert.AreEqual(true, clientConnected);

            const int length = 100000;
            var charArray = new char[length];
            charArray[0] = 'a';
            for (int i = 1; i < length-1; ++i) charArray[i] = 'b';
            charArray[length-1] = 'c';
            var testString = new string(charArray);

            // how to set a timeout? 

            var response = (await _testServiceClient.SendMessage(testString)) as TestServiceResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(TestServiceConstants.ServicePrefix + testString, response.Message);

            _testServiceClient.StopClient();
            // assert client is stopped?
            _testService.StopService();
        }

        [TestMethod]
        public async Task TestMultipleClients()
        {
            _testService = new TestService(_port);
            Assert.AreEqual(true, _testService.StartService());

            const int numClients = 5;
            TestServiceClient[] clients = new TestServiceClient[numClients];
            for (int i = 0; i < numClients; ++i)
            {
                clients[i] = new TestServiceClient(_serverAddress, _port);
                bool clientConnected = await clients[i].StartConnection();
                Assert.AreEqual(true, clientConnected);
            }

            for (int i = 0; i < numClients; ++i)
                await SendAndReceive(clients[i], i);

            for (int i = 0; i < numClients; ++i)
            {
                clients[i].StopClient();
            }
            _testService.StopService();
        }

        private async Task SendAndReceive(TestServiceClient client, int i)
        {
                string testString = i.ToString();
                var response = (await client.SendMessage(testString)) as TestServiceResponse;
                Assert.IsNotNull(response);
                Assert.AreEqual(TestServiceConstants.ServicePrefix + testString, response.Message);
        }

        [TestMethod]
        public async Task TestClientReConnect()
        {
            _testService = new TestService(_port);
            Assert.AreEqual(true, _testService.StartService());

            // first connection
            _testServiceClient = new TestServiceClient(_serverAddress, _port);
            bool clientConnected = await _testServiceClient.StartConnection();
            Assert.AreEqual(true, clientConnected);

            const string testString = "Hello world!";
            var response = (await _testServiceClient.SendMessage(testString)) as TestServiceResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(TestServiceConstants.ServicePrefix + testString, response.Message);

            _testServiceClient.StopClient();

            // second connection
            clientConnected = await _testServiceClient.StartConnection();
            Assert.AreEqual(true, clientConnected);
            response = (await _testServiceClient.SendMessage(testString)) as TestServiceResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(TestServiceConstants.ServicePrefix + testString, response.Message);

            _testServiceClient.StopClient();
            _testService.StopService();
        }
    }
}
