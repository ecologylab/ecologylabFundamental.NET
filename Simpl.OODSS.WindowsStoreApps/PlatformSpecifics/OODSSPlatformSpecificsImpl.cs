using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.Web;

namespace Simpl.OODSS.PlatformSpecifics
{
    public class OODSSPlatformSpecificsImpl : IOODSSPlatformSpecifics
    {
        /// <summary>
        /// WindowsStoreApp Implementation
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// 
        /// 
        private StreamWebSocket _streamWebSocket;
        
        public object CreateWebSocketClientObject()
        {
            StreamWebSocket webSocket =  new StreamWebSocket();
            _streamWebSocket = webSocket;
            webSocket.Closed += Closed;
            return webSocket;
        }

        public void DisconnectWebSocketClient(object webSocketClient)
        {
            var websocket = webSocketClient as StreamWebSocket;
            if (websocket == null)
            {
                throw new InvalidCastException("cannot cast webSocketClient object to StreamWebSocket");
            }
            websocket.Close(1000, "GoodBye");
        }

        public bool WebSocketIsConnected(object webSocketClient)
        {
            var websocket = webSocketClient as StreamWebSocket;
            if (websocket != null)
            {
                return true;// This is not correct. how to get the connection status of websocket
            }
            throw new InvalidCastException("cannot cast webSocketClient object to StreamWebSocket");
        }
        

        public async Task ConnectWebSocketClientAsync(object webSocketClient, Uri uri, CancellationToken token)
        {
            // Make a local copy to avoid races with the Closed event.
            StreamWebSocket webSocket = webSocketClient as StreamWebSocket;
            if (webSocket == null)
            {
                throw new InvalidCastException("cannot cast webSocketClient object to StreamWebSocket");
            }
            try
            {
                await _streamWebSocket.ConnectAsync(uri);
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine("Exception during connecting: {0}", ex.Message);
                throw;
            }
           
        }

        public async Task SendMessageFromWebSocketClientAsync(object webSocketClient, byte[] outMessage)
        {
            var websocket = webSocketClient as StreamWebSocket;
            if (websocket == null)
            {
                throw new InvalidCastException("cannot cast webSocketClient object to StreamWebSocket");
            }
            try
            {
                IOutputStream writeStream = websocket.OutputStream;
                await writeStream.WriteAsync(outMessage.AsBuffer());
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine("Exception during message sending: " + ex.Message);
                throw;
            }
        }

        public async Task ReceiveMessageFromWebSocketClientAsync(object webSocketClient, byte[] buffer, CancellationToken token)
        {
            var websocket = webSocketClient as StreamWebSocket;
            if (websocket == null)
            {
                throw new InvalidCastException("cannot cast webSocketClient object to StreamWebSocket");
            }
            try
            {
                IInputStream readStream = websocket.InputStream;
                await readStream.ReadAsync(buffer.AsBuffer(), Convert.ToUInt16(buffer.Length), InputStreamOptions.Partial);
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine("Exception during message sending: " + ex.Message);
                throw;
            }
        }

        public void CreateWorkingThreadAndStart(Action sender, Action receiver, Action dataReceiver, CancellationToken token)
        {
            ThreadPool.RunAsync(delegate { sender.Invoke(); });
            ThreadPool.RunAsync(delegate { sender.Invoke(); });
            ThreadPool.RunAsync(delegate { sender.Invoke(); });
        }

        private void Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            // You can add code to log or display the code and reason
            // for the closure (stored in args.code and args.reason)

            // This is invoked on another thread so use Interlocked 
            // to avoid races with the Start/Stop/Reset methods.
            StreamWebSocket webSocket = Interlocked.Exchange(ref _streamWebSocket, null);
            if (webSocket != null)
            {
                webSocket.Dispose();
            }

        }
    }
}
