using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simpl.OODSS.PlatformSpecifics
{
    public interface IOODSSPlatformSpecifics
    {
        /// <summary>
        /// Create a websocket client object for the specific platform
        /// in windowsRT, returns Windows.Networking.Sockets.StreamWebSocket
        /// in .NET, returns System.Net.WebSockets.ClientWebSocket
        /// </summary>
        /// <returns></returns>
        object CreateWebSocketClientObject();

        /// <summary>
        /// Disconnect the websocket client
        /// </summary>
        /// <param name="webSocketClient"></param>
        /// <returns></returns>
        void DisconnectWebSocketClient(object webSocketClient);

        /// <summary>
        /// Check whether websocket client is connected
        /// </summary>
        /// <param name="webSocketClient"></param>
        /// <returns></returns>
        bool WebSocketIsConnected(object webSocketClient);

        /// <summary>
        /// Connect the websocket client
        /// </summary>
        Task ConnectWebSocketClientAsync(object webSocketClient, Uri uri, CancellationToken token);

        /// <summary>
        /// Send bytes through webSocket client
        /// </summary>
        /// <param name="webSocketClient"></param>
        /// <param name="outMessage"></param>
        Task SendMessageFromWebSocketClientAsync(object webSocketClient, byte[] outMessage);



        Task ReceiveMessageFromWebSocketClientAsync(object webSocketClient, byte[] buffer, CancellationToken token);


        void CreateWorkingThreadAndStart(Action sender, Action receiver, Action dataReceiver, CancellationToken token);
    }
}
