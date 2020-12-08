using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using LegendsServer.Core;

namespace LegendsServer
{
    class SocketServer
    {
        public bool Listening = true;

        public void Start()
        {
            var thread = new Thread(new ThreadStart(Listen));
            thread.Start();
        }

        public async void Listen()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://+:8080/server/");
            listener.Start();
            Console.WriteLine("Networking listening...");
            while (Listening)
            {
                HttpListenerContext listenerContext = await listener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    ProcessRequest(listenerContext);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }

        private async void ProcessRequest(HttpListenerContext listenerContext)
        {
            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
            }
            catch (Exception e)
            {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Console.WriteLine("Exception: {0}", e);
                return;
            }

            WebSocket webSocket = webSocketContext.WebSocket;
            try
            {
                byte[] receiveBuffer = new byte[1024];
                //var stream = new GameStream(receiveBuffer);
                var player = new PlayerConnection(webSocket);

                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    // Received Text Message
                    else if (receiveResult.MessageType == WebSocketMessageType.Text)
                        await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept text frame", CancellationToken.None);
                    else
                        //PacketHandler.PacketsToClientEvents(player, stream);
                        Console.WriteLine("Received Packet");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
            }
        }
    }

    public static class HelperExtensions
    {
        public static Task GetContextAsync(this HttpListener listener)
        {
            return Task.Factory.FromAsync<HttpListenerContext>(listener.BeginGetContext, listener.EndGetContext, TaskCreationOptions.None);
        }
    }
}