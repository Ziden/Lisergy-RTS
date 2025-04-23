using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Engine.Network;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Telepathy;

[assembly: InternalsVisibleTo("ServerTests")]

namespace ClientSDK;

public class ClientNetwork : IGameNetwork, IEventListener
{
	internal const int READS_PER_TICK = 5;
	internal Dictionary<ServerType, Client> _connections = new();
	private LoginResultPacket _credentials = null!;

	private bool _enabled = true;
	private readonly IGameLog _log;
	internal Message _msg;
	internal EventBus<BasePacket> _receivedFromServer = new();
	internal HashSet<ServerType> _serversConnected = new();
	internal Dictionary<ServerType, Queue<byte[]>> _toSend = new();

	public ClientNetwork(IGameLog log, params ServerType[] connections)
	{
		_log = log;
		foreach (var server in connections)
		{
			_connections[server] = new Client();
			_toSend[server] = new Queue<byte[]>();
		}

		DeltaCompression = new DeltaCompression(null);
		log.Info("Created client network");
	}

	public IReadOnlyCollection<ServerType> ServersConnected => _serversConnected;

	public DeltaCompression DeltaCompression { get; }

	public void SetupGame(IGame game)
	{
	}

	public void OnInput<T>(Action<T> listener) where T : BasePacket
	{
		_receivedFromServer.On(this, listener);
	}

	public void ReceiveInput(GameId sender, BasePacket input)
	{
		_log.Debug($"Received Packet {input}");
		_receivedFromServer.Call(input);
	}

	public void SendToPlayer<PacketType>(PacketType p, params GameId[] players) where PacketType : BasePacket
	{
		throw new Exception("P2P Not Enabled - yet");
	}

	public void SendToServer(BasePacket p, ServerType server = ServerType.WORLD)
	{
		_enabled = true;
		_log.Debug($"Sending {p.GetType().Name} to {server}");
		OnSendGenericPacket?.Invoke(p);
		_toSend[server].Enqueue(Serialization.FromAnyType(p).ToArray());
	}

	public event Action<BasePacket>? OnReceiveGenericPacket;
	public event Action<BasePacket>? OnSendGenericPacket;

    /// <summary>
    ///     Whenever we receive credentials we use it to handshake with the servers
    /// </summary>
    public void SetCredentials(LoginResultPacket loginResult)
	{
		if (!loginResult.Success) return;
		_credentials = loginResult;
		_log.Info("Logged In - Sending Handshake to servers");
		var handshake = new HandshakePacket
		{
			PlayerId = loginResult.Profile.PlayerId,
			Token = loginResult.Token
		};
		SendToServer(handshake);
		SendToServer(handshake, ServerType.CHAT);
	}

	public void ReceiveServerMessage(ServerType server, BasePacket input)
	{
		ReceiveInput(GameId.ZERO, input);
		OnReceiveGenericPacket?.Invoke(input);
	}

	public void Disconnect()
	{
		_log.Info("Client disconnecting");
		_enabled = false;
		foreach (var client in _connections.Values) client.Disconnect();
	}

	public void Tick()
	{
		if (!_enabled) return;
		foreach (var (server, socket) in _connections)
		{
			if (!socket.Connected)
			{
				socket.Connect("127.0.0.1", server.GetDefaultPort());
				return;
			}

			while (_toSend[server].TryDequeue(out var data)) socket.Send(data);

			for (var x = 0; x < READS_PER_TICK; x++)
			{
				if (!socket.GetNextMessage(out _msg))
					break;

				switch (_msg.eventType)
				{
					case EventType.Connected:
						_log.Debug($"Connected to Server {server}");
						_serversConnected.Add(server);
						break;
					case EventType.Data:
						ReceiveServerMessage(server, Serialization.ToAnyType<BasePacket>(_msg.data));
						break;
					case EventType.Disconnected:
						_serversConnected.Remove(server);
						_log.Debug($"Disconnected from Server {server}");
						break;
				}
			}
		}
	}
}