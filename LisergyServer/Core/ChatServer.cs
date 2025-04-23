using System;
using System.Collections.Generic;
using BaseServer.Commands;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Network.ClientPackets;
using LisergyServer.Core;

namespace MapServer;

/// <summary>
///     Simple chat server for now, no channels or message types just raw simple global chat
/// </summary>
public class ChatServer : BaseHandshackedServer
{
	private readonly List<ChatPacket> _chatLog = new();

	public override ServerType GetServerType()
	{
		return ServerType.CHAT;
	}

	public override void RegisterConsoleCommands(ConsoleCommandExecutor executor)
	{
	}

	public override void Tick()
	{
	}

	public override void OnReceiveAuthenticatedPacket(in GameId player, BasePacket packet)
	{
		if (packet is ChatPacket chatPacket)
		{
			chatPacket.Owner = player;
			chatPacket.Time = DateTime.UtcNow;
			Log.Debug($"ChatPacket From {player}: {chatPacket.Name} " + chatPacket.Message);
			_chatLog.Add(chatPacket);
			if (_chatLog.Count > 30) _chatLog.RemoveAt(0);
			_validConnections.Broadcast(chatPacket);
		}
	}

	public override void OnAuthenticated(ConnectedPlayer player)
	{
		Log.Debug("Sending chat log to player " + player.PlayerId);
		player.Send(new ChatLogPacket
		{
			Messages = _chatLog
		});
	}
}