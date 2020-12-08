using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace LegendsServer
{
    public class PlayerConnection
    {
        public WebSocket Socket;

        public Guid Id;

        public PlayerConnection(WebSocket ws)
        {
            Socket = ws;
        }

        public void SendPacket()
        {
            //ArraySegment<byte> buffer;
            //stream.TryGetBuffer(out buffer);
            //Socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public void PopupMessage(string text)
        {
            var encoded = Encoding.UTF8.GetBytes(text);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void Disconnect(string reason)
        {
            PopupMessage(reason);
            Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
        }

        public override string ToString()
        {
            return $"<Player id={Id.ToString()}>";
        }
    }
}
