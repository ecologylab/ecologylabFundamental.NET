using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Simpl.OODSS.TestClientAndMessage.Client;
using Simpl.OODSS.TestClientAndMessage.Messages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Simpl.OODSS.WinRT.Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private int _port = 2018;
        private string _serverAddress = "localhost";

        private TestServiceClient _testServiceClient;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void TestConnectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            InfoLabel.Text = "Running...";
            _testServiceClient = new TestServiceClient(_serverAddress, _port);
            bool clientConnected = await _testServiceClient.StartConnection();
            InfoLabel.Text = clientConnected ? "Pass" : "Fail";

            _testServiceClient.StopClient();
        }

        private async void TestLargeSizeMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            InfoLabel.Text = "Running...";
            _testServiceClient = new TestServiceClient(_serverAddress, _port);
            bool clientConnected = await _testServiceClient.StartConnection();

            const int length = 30000;
            var charArray = new char[length];
            charArray[0] = 'a';
            for (int i = 1; i < length - 1; ++i) charArray[i] = 'b';
            charArray[length - 1] = 'c';
            var testString = new string(charArray);

            var response = (await _testServiceClient.SendMessage(testString)) as TestServiceResponse;

            if (response != null && response.Message.Equals(TestServiceConstants.ServicePrefix + testString))
                InfoLabel.Text = "Pass";
            else
                InfoLabel.Text = "Fail";
            _testServiceClient.StopClient();
        }

        private async void TestReconnectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            InfoLabel.Text = "Running...";
            _testServiceClient = new TestServiceClient(_serverAddress, _port);
            bool clientConnected = await _testServiceClient.StartConnection();
            if (!clientConnected)
            {
                InfoLabel.Text = "Fail";
                return;
            }
            var testString = "Hello world!";
            var response = (await _testServiceClient.SendMessage(testString)) as TestServiceResponse;
            _testServiceClient.StopClient();

            clientConnected = await _testServiceClient.StartConnection();
            if (!clientConnected)
            {
                InfoLabel.Text = "Fail";
                return;
            }
            response = (await _testServiceClient.SendMessage(testString)) as TestServiceResponse;

            if (response != null && response.Message.Equals(TestServiceConstants.ServicePrefix + testString))
                InfoLabel.Text = "Pass";
            else
                InfoLabel.Text = "Fail";

            _testServiceClient.StopClient();
        }

        private async void TestMultiClientButton_OnClick(object sender, RoutedEventArgs e)
        {
            InfoLabel.Text = "Running...";
            const int numClients = 5;
            TestServiceClient[] clients = new TestServiceClient[numClients];
            for (int i = 0; i < numClients; ++i)
            {
                clients[i] = new TestServiceClient(_serverAddress, _port);
                bool clientConnected = await clients[i].StartConnection();
                if (!clientConnected)
                {
                    InfoLabel.Text = "Fail";
                    return;
                }
            }

            for (int i = 0; i < numClients; ++i)
            {
                bool success = await SendAndReceive(clients[i], i);
                if (!success)
                {
                    InfoLabel.Text = "Fail";
                    return;
                }
            }

            for (int i = 0; i < numClients; ++i)
            {
                clients[i].StopClient();
            }
            InfoLabel.Text = "Pass";
        }

        private async Task<bool> SendAndReceive(TestServiceClient client, int i)
        {
            string testString = i.ToString();
            var response = (await client.SendMessage(testString)) as TestServiceResponse;
            return response != null && response.Message.Equals(TestServiceConstants.ServicePrefix + testString);
        }
    }
}
