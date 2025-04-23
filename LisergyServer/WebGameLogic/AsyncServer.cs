using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Events.ServerEvents;
using Game.Services;
using WebPlayerLogic.Playfab;

namespace MapServer;

/// <summary>
///     Unified one server for everything, for now
/// </summary>
public class AsyncServer
{
	private readonly string _playerId;
	private BattleService? _battleService;
	private readonly GameServerNetwork? _network;
	private readonly EntityPersistence? _persistence;
	private WorldService? _worldService;

	public AsyncServer(string playerId, LisergyGame game)
	{
		_playerId = playerId ?? throw new ArgumentNullException(nameof(playerId));
		Game = game ?? throw new ArgumentNullException(nameof(game));

		Serialization.LoadSerializers();
		_network = game.Network as GameServerNetwork;

		if (_network != null)
		{
			_battleService = new BattleService(Game);
			_worldService = new WorldService(Game);
			_persistence = new EntityPersistence(game);
			_network.OnOutgoingPacket += SendPacketToPlayer;
		}
	}

	public LisergyGame Game { get; }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
	protected async void SendPacketToPlayer(GameId player, BasePacket packet)
	{
		try
		{
			if (packet is EntityUpdatePacket e && _persistence != null)
			{
				var entity = Game.Entities[e.EntityId];
				if (entity != null) await _persistence.SaveEntity(_playerId, entity);
			}
		}
		catch (Exception ex)
		{
			// Log error or handle exception
			Console.WriteLine($"Error in SendPacketToPlayer: {ex.Message}");
		}
	}
#pragma warning restore CS4014
}