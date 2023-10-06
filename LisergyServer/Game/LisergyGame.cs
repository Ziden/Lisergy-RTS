
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.Network;
using Game.Systems.Player;
using Game.World;
using GameData;

namespace Game
{
    public interface IGame
    {
        public GameSpec Specs { get; }
        public IGameEntities Entities { get; }
        public IGamePlayers Players { get; }
        public IGameWorld GameWorld { get; }
        public ISystems Systems { get; }
        public IGameLogic Logic { get; }
        public EventBus<BaseEvent> NetworkPackets { get; }
        public EventBus<GameEvent> Events { get; }
    }

    public class LisergyGame : IGame
    {
        public GameSpec Specs { get; private set; }
        public GameWorld World { get; private set; }
        public IGameEntities Entities { get; private set; }
        public IGameWorld GameWorld => World;
        public ISystems Systems { get; private set; }
        public IGamePlayers Players => World.Players;
        public IGameLogic Logic { get; private set; }
        public IEntityLogic EntityLogic(IEntity e) => Logic.EntityLogic(e);
        public EventBus<BaseEvent> NetworkPackets { get; private set; } = new EventBus<BaseEvent>();
        public EventBus<GameEvent> Events { get; private set; } = new EventBus<GameEvent>();
        public void SetWorld(GameWorld world)
        {
            world.Game = this;
            World = world;
            Entities = new GameEntities(this);
            Systems = new GameSystems(this);
            Logic = new GameLogic(Systems);
        }

        public void ReceiveInput(PlayerEntity sender, byte[] input)
        {
            BaseEvent ev = Serialization.ToEventRaw(input);
            ev.Sender = sender;
            NetworkPackets.Call(ev);
            DeltaTracker.SendDeltaPackets(sender);
        }

        public LisergyGame(GameSpec specs)
        {
            Specs = specs;
        }

        public void ClearEventListeners()
        {
            NetworkPackets.Clear();
        }
    }
}
