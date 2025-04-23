using System;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using GameServices;
using Telepathy;

namespace LisergyServer.Core;

public class ConnectedPlayer : IConnectedPlayer
{
	private GameId _playerId;

	public ConnectedPlayer(in GameId playerid, in int connectionId, Server server)
	{
		PlayerId = playerid;
		_server = server;
		ConnectionID = connectionId;
	}

	private Server _server { get; }
	public int ConnectionID { get; set; }
	public ref GameId PlayerId => ref _playerId;

	public void Send<PacketType>(PacketType ev) where PacketType : BasePacket, new()
	{
		var bytes = Serialization.FromAnyType(ev);
		PacketPool.Return(ev);
		SendBytes(bytes);
	}

	public void SendBytes(in ReadOnlyMemory<byte> data)
	{
		_server.Send(ConnectionID, data.ToArray());
	}


	public virtual bool Online()
	{
		return _server.GetClientAddress(ConnectionID) != "";
	}
}