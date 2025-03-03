using ClientSDK.SDKEvents;
using Game.Engine;
using Game.Network.ClientPackets;
using Game.Systems.Player;
using System.Collections.Generic;
using System.Linq;

namespace ClientSDK.Modules
{
    /// <summary>
    /// Allows player to send chat messages. Will handle receiving messages too.
    /// </summary>
    public interface IChatModule : IClientModule
    {
        /// <summary>
        /// Gets the last two messages of the chat
        /// </summary>
        public ChatPacket[] GetThumbnail();

        /// <summary>
        /// Gets the full chat history
        /// </summary>

        public IReadOnlyCollection<ChatPacket> GetFullChat();

        /// <summary>
        /// Sends a message to chat
        /// </summary>
        public void SendMessage(string message);
    }

    class ChatSorter : IComparer<ChatPacket>
    {
        public int Compare(ChatPacket x, ChatPacket y) => x.Time.CompareTo(y.Time);
    }

    public class ChatModule : IChatModule
    {
        private const int MAX_SIZE = 5;

        private SortedSet<ChatPacket> _chatLog = new SortedSet<ChatPacket>(new ChatSorter());

        private GameClient _gameClient;

        public ChatModule(GameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public IReadOnlyCollection<ChatPacket> GetFullChat()
        {
            return _chatLog;
        }

        public ChatPacket[] GetThumbnail()
        {
            return new ChatPacket[]
            {
                _chatLog.ElementAt(0), _chatLog.ElementAt(1)
            };
        }

        public void Register()
        {
            _gameClient.Network.OnInput<ChatPacket>(OnChat);
            _gameClient.Network.OnInput<ChatLogPacket>(OnChatLog);
        }

        private void OnChatLog(ChatLogPacket chatLog)
        {
            _chatLog.Clear();
            foreach (var c in chatLog.Messages) _chatLog.Add(c);
            _gameClient.ClientEvents.Call(new ChatUpdateEvent());
        }

        private void OnChat(ChatPacket packet)
        {
            _chatLog.Add(packet);
            if (_chatLog.Count > MAX_SIZE) _chatLog.Remove(_chatLog.Last());
            _gameClient.ClientEvents.Call(new ChatUpdateEvent()
            {
                NewPacket = packet
            });
        }

        public void SendMessage(string message)
        {
            _gameClient.Network.SendToServer(new ChatPacket()
            {
                Name = _gameClient.Modules.Player.LocalPlayer.GetFromEntity<PlayerProfileComponent>().Name,
                Message = message,
            }, ServerType.CHAT);
        }
    }
}
