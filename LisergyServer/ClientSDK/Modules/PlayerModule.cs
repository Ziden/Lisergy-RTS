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
    /// Will also fire events such as OwnEntityInfoReceived for local player data updates
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

    public class PlayerModule(LisergySDK client) : IPlayerModule
    {
        public PlayerModel LocalPlayer { get; private set; } = null!;
        public GameId PlayerId => LocalPlayer.EntityId;

        public void Register()
        {
            client.ClientEvents.On<GameStartedEvent>(this, OnGameStart);
            client.ClientEvents.On<EntityViewRendered>(this, OnAwareOfEntity);
            client.Network.OnInput<BattleHeaderPacket>(OnBattleSummary);
        }

        private void OnBattleSummary(BattleHeaderPacket result)
        {
            LocalPlayer.Components.Get<PlayerDataComponent>().BattleHeaders.Add(result.BattleHeader);
        }

        private void OnAwareOfEntity(EntityViewRendered ev)
        {
            if (ev.Entity.OwnerID != PlayerId) return;
            client.Game.Entities.SetParent(LocalPlayer.EntityId, ev.Entity.EntityId);
            client.ClientEvents.Call(new OwnEntityInfoReceived(ev.Entity));
        }

        private void OnGameStart(GameStartedEvent gameStartedEvent)
        {
            LocalPlayer = gameStartedEvent.LocalPlayer;
            client.Game.Players.Add(LocalPlayer);
        }
    }
}
