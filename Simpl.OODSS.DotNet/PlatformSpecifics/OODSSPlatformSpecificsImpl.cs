using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simpl.OODSS.PlatformSpecifics
{
    /// <summary>
    /// DotNet Implementation
    /// </summary>
    public class OODSSPlatformSpecificsImpl : IOODSSPlatformSpecifics
    {
        public object CreateWebSocketClientObject()
        {
            try
            {
                return new ClientWebSocket();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: {0}", ex.Message);
                throw;
            }
        }

        public async Task DisconnectWebSocketClientAsync(object webSocketClient)
        {
            var websocket = webSocketClient as ClientWebSocket;
            if (websocket == null)
            {
                throw new InvalidCastException("Cannot cast webSocketClient object to ClientWebSocket");
            }
            await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);;
        }

        public bool WebSocketIsConnected(object webSocketClient)
        {
            var websocket = webSocketClient as ClientWebSocket;
            if (websocket != null)
            {
                if (websocket.State == WebSocketState.Open)
                    return true;
                // TODO : what if == WebSocketState.Connecting?
                return false;
            }
            throw new InvalidCastException("Cannot cast webSocketClient object to ClientWebSocket");
        }

        public async Task ConnectWebSocketClientAsync(object webSocketClient, Uri uri, CancellationToken token)
        {
            var websocket = webSocketClient as ClientWebSocket;
            if (websocket == null)
            {
                throw new InvalidCastException("Cannot cast webSocketClient object to ClientWebSocket");
            }
            try
            {
                await websocket.ConnectAsync(uri, token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception during connecting: {0}", ex.Message);
                throw;
            }   
        }

        public async Task SendMessageFromWebSocketClientAsync(object webSocketClient, byte[] outMessage)
        {    
            var websocket = webSocketClient as ClientWebSocket;
            if (websocket == null)
            {
                throw new InvalidCastException("Cannot cast webSocketClient object to ClientWebSocket");    
            }
            try
            {
                await websocket.SendAsync(new ArraySegment<byte>(outMessage), WebSocketMessageType.Binary, true,
                    CancellationToken.None);
                Debug.WriteLine("message sent");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception during message sending: " + ex.Message);
                throw;
            }
        }

        public async Task<byte[]> ReceiveMessageFromWebSocketClientAsync(object webSocketClient, byte[] buffer, CancellationToken token)
        {
            var websocket = webSocketClient as ClientWebSocket;
            if (websocket == null)
            {
                throw new InvalidCastException("Cannot cast webSocketClient object to ClientWebSocket");
            }
            try
            {
                // TODO: make sure it is the same as the WinRT streamWebSocket
                WebSocketReceiveResult receiveResult = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                Debug.WriteLine("receive message");
                var lengthByte = new byte[4];
                Array.Copy(buffer, lengthByte, 4);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lengthByte);
                var length = BitConverter.ToInt32(lengthByte, 0);
                var incomingData = new byte[length];
                Debug.WriteLine("message length: {0}", length);

                int received = receiveResult.Count - 4;
                Array.Copy(buffer, 4, incomingData, 0, receiveResult.Count - 4);

                while (!receiveResult.EndOfMessage)
                {
                    receiveResult = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    Array.Copy(buffer, 0, incomingData, received, receiveResult.Count);
                    received += receiveResult.Count;
                }

                Debug.WriteLine(received);
                return incomingData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception during websocket data receiving: " + ex.Message);
                throw;
            }
        }

        public void CreateWorkingThreadAndStart(Action sender, Action receiver, Action dataReceiver, CancellationToken token)
        {
            Thread senderThread = new Thread(new ThreadStart(sender));
            Thread receiverThread = new Thread(new ThreadStart(receiver));
            Thread dataReceiverThread = new Thread(new ThreadStart(dataReceiver));
            senderThread.Name = "SenderThread";
            receiverThread.Name = "ReceiverThread";
            dataReceiverThread.Name = "DataReceiverThread";
            senderThread.Start();
            receiverThread.Start();
            dataReceiverThread.Start();
        }
    }
}
