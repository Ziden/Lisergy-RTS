using ClientSDK.SDKEvents;
using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Events;
using Game.Systems.Player;


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
        public PlayerModel LocalPlayer { get; }
    }

    public class PlayerModule : IPlayerModule
    {
        private GameClient _client;
        public PlayerModel LocalPlayer { get; private set; }
        public GameId PlayerId => LocalPlayer.EntityId;
        public PlayerModule(GameClient client) { _client = client; }

        public void Register()
        {
            _client.ClientEvents.On<GameStartedEvent>(this, OnGameStart);
            _client.ClientEvents.On<EntityViewCreated>(this, OnAwareOfEntity);
            _client.Network.OnInput<BattleHeaderPacket>(OnBattleSummary);
        }

        private void OnBattleSummary(BattleHeaderPacket result)
        {
            LocalPlayer.Components.Get<PlayerDataComponent>().BattleHeaders.Add(result.BattleHeader);
        }

        private void OnAwareOfEntity(EntityViewCreated ev)
        {
            if (ev.Entity.OwnerID != PlayerId) return;
            _client.Game.Entities.SetParent(LocalPlayer.EntityId, ev.Entity.EntityId);
            _client.ClientEvents.Call(new OwnEntityInfoReceived(ev.Entity));
        }

        private void OnGameStart(GameStartedEvent gameStartedEvent)
        {
            LocalPlayer = gameStartedEvent.LocalPlayer;
            _client.Game.Players.Add(LocalPlayer);
        }
    }
}
