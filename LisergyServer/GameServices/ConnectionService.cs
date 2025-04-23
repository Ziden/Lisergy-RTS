using System;
using System.Collections.Generic;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;

namespace GameServices;

public interface IConnectedPlayer
{
	public ref GameId PlayerId { get; }
	int ConnectionID { get; }
	void Send<PacketType>(PacketType ev) where PacketType : BasePacket, new();
	void SendBytes(in ReadOnlyMemory<byte> data);
}

public class ConnectionService
{
	private readonly Dictionary<int, IConnectedPlayer> _connectedByConnectionId = new();
	private readonly Dictionary<GameId, IConnectedPlayer> _connectedById = new();
	private readonly IGameLog _log;

	public ConnectionService(IGameLog log)
	{
		_log = log;
	}

	public void Broadcast(BasePacket packet)
	{
		var bytes = Serialization.FromAnyType((object) packet).ToArray();
		foreach (var u in _connectedById.Values)
		{
			_log.Debug($"Broadcasting {packet.GetType()} to player {u.PlayerId}");
			u.SendBytes(bytes);
		}
	}

	public void RegisterAuthenticatedConnection(IConnectedPlayer user)
	{
		_connectedById[user.PlayerId] = user;
		_connectedByConnectionId[user.ConnectionID] = user;
		_log.Debug($"Player {user.PlayerId} Registered connection id {user.ConnectionID}");
	}

	public void Disconnect(in int connectionId)
	{
		if (_connectedByConnectionId.TryGetValue(connectionId, out var user))
		{
			_log.Debug($"Player {user.PlayerId} disconnected from connection id {connectionId}");
			_connectedById.Remove(user.PlayerId);
			_connectedByConnectionId.Remove(user.ConnectionID);
		}
		else
		{
			_log.Error($"Error disconnecting connection {connectionId} - unknown user");
		}
	}

	public bool IsConnectionAuthenticated(in int connection)
	{
		return _connectedByConnectionId.ContainsKey(connection);
	}

	public IConnectedPlayer GetAuthenticatedConnection(in int connection)
	{
		return _connectedByConnectionId[connection];
	}

	public IConnectedPlayer? GetConnectedPlayer(in GameId id)
	{
		_connectedById.TryGetValue(id, out var connected);
		return connected;
	}
}