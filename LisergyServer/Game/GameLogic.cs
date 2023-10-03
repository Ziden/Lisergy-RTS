
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.Network;
using Game.Systems.Player;
using GameData;

namespace Game
{
    public interface IGameLogic
    {
        public IGameWorld GameWorld { get; }
        public ISystems Systems { get; }
        public EventBus<BaseEvent> NetworkPackets { get; }
        public EventBus<GameEvent> Events { get; }
    }

    public class GameLogic
    {
        public static ref GameSpec Specs => ref _spec;
        private static GameSpec _spec;
        public GameWorld World { get; private set; }
        public IGameWorld GameWorld => World;
        public ISystems Systems { get; private set; }

        public EventBus<BaseEvent> NetworkPackets { get; private set; } = new EventBus<BaseEvent>();
        public EventBus<GameEvent> Events { get; private set; } = new EventBus<GameEvent>();

        public void ReceiveInput(PlayerEntity sender, byte[] input)
        {
            BaseEvent ev = Serialization.ToEventRaw(input);
            ev.Sender = sender;
            NetworkPackets.Call(ev);
            DeltaTracker.SendDeltaPackets(sender);
        }

        public void SetWorld(GameWorld world)
        {
            world.Game = this;
            World = world;
            Systems = new GameSystems(this);
        }

        public GameLogic(in GameSpec specs)
        {
            _spec = specs;
        }

        public GameLogic(in GameSpec specs, GameWorld world)
        {
            SetWorld(world);
            _spec = specs;
        }

        public void ClearEventListeners()
        {
            NetworkPackets.Clear();
        }
    }
}
