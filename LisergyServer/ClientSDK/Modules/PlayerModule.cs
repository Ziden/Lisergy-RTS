using ClientSDK.SDKEvents;
using Game.DataTypes;
using Game.Events;
using Game.Events.Bus;
using Game.Systems.Building;
using Game.Systems.Party;
using Game.Systems.Player;
using System.IO;


namespace ClientSDK.Services
{

    /// <summary>
    /// Keeps track of the local player data.
    /// Will listen to events and packets to enrich the local player data.
    /// Responsibility of this module is to listen to game events and keep local player data up-to date
    /// Will also fire events such as <see cref="OwnEntityInfoReceived<T>" for local player data updates/>
    /// </summary>
    public interface IPlayerModule : IClientModule, IEventListener
    {
        /// <summary>
        /// Gets the local player user id
        /// </summary>
        public GameId PlayerId { get; }

        /// <summary>
        /// Gets the local player reference.
        /// Shall contain all data the client is aware of
        /// </summary>
        public PlayerEntity LocalPlayer { get; }    
    }

    public class PlayerModule : IPlayerModule
    {
        private IGameClient _client;
        public PlayerEntity LocalPlayer { get; private set; }
        public GameId PlayerId => LocalPlayer.EntityId;

        public PlayerModule(IGameClient client) { _client = client; }

        public void Register()
        {
            _client.ClientEvents.Register<GameStartedEvent>(this, OnGameStart);
            _client.ClientEvents.Register<ClientAwareOfEntityEvent>(this, OnAwareOfEntity);
            _client.Network.On<BattleHeaderPacket>(OnBattleSummary);
        }

        private void OnBattleSummary(BattleHeaderPacket result)
        {
            LocalPlayer.Data.BattleHeaders.Add(result.BattleHeader);
        }

        private void OnAwareOfEntity(ClientAwareOfEntityEvent ev) {
            if (ev.Entity.OwnerID != PlayerId) return;
            LocalPlayer.AddOwnedEntity(ev.Entity);
            if(ev.Entity is PlayerBuildingEntity building)
            {
                _client.ClientEvents.Call(new OwnEntityInfoReceived<PlayerBuildingEntity>(building));
            } else if(ev.Entity is PartyEntity party)
            {
                _client.ClientEvents.Call(new OwnEntityInfoReceived<PartyEntity>(party));
            }
        }

        private void OnGameStart(GameStartedEvent gameStartedEvent)
        {
            LocalPlayer = gameStartedEvent.LocalPlayer;
        }
    }
}
