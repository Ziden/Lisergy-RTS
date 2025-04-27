using System.Collections.Generic;
using System.Linq;
using ClientSDK.SDKEvents;
using Game.Engine;
using Game.Network.ClientPackets;
using Game.Systems.Player;

namespace ClientSDK.Modules;

/// <summary>
///     Allows player to send chat messages. Will handle receiving messages too.
/// </summary>
public interface IChatModule : IClientModule
{
    /// <summary>
    ///     Gets the last two messages of the chat
    /// </summary>
    public ChatPacket[] GetThumbnail();

    /// <summary>
    ///     Gets the full chat history
    /// </summary>
    public IReadOnlyCollection<ChatPacket> GetFullChat();

    /// <summary>
    ///     Sends a message to chat
    /// </summary>
    public void SendMessage(string message);
}

internal class ChatSorter : IComparer<ChatPacket>
{
	public int Compare(ChatPacket? x, ChatPacket? y)
	{
		if (x == null && y == null) return 0;
		if (x == null) return -1;
		if (y == null) return 1;

		return x.Time.CompareTo(y.Time);
	}
}

public class ChatModule : IChatModule
{
	private const int MAX_SIZE = 5;

	private readonly SortedSet<ChatPacket> _chatLog = new(new ChatSorter());

	private readonly LisergySDK _gameClient;

	public ChatModule(LisergySDK gameClient)
	{
		_gameClient = gameClient;
	}

	public IReadOnlyCollection<ChatPacket> GetFullChat()
	{
		return _chatLog;
	}

	public ChatPacket[] GetThumbnail()
	{
		return new[]
		{
			_chatLog.ElementAt(0), _chatLog.ElementAt(1)
		};
	}

	public void Register()
	{
		_gameClient.Network.OnInput<ChatPacket>(OnChat);
		_gameClient.Network.OnInput<ChatLogPacket>(OnChatLog);
	}

	public void SendMessage(string message)
	{
		_gameClient.Network.SendToServer(new ChatPacket
		{
			Name = _gameClient.Modules.Player.LocalPlayer.GetFromEntity<PlayerProfileComponent>().Name,
			Message = message
		}, ServerType.CHAT);
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
		_gameClient.ClientEvents.Call(new ChatUpdateEvent
		{
			NewPacket = packet
		});
	}
}