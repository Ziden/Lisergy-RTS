
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.Network;
using Game.Player;
using GameData;
using System.Collections.Generic;

namespace Game
{
    public class StrategyGame
    {

        private static GameSpec _spec;
        public static ref GameSpec Specs => ref _spec;

        public ref GameSpec GameSpec => ref _spec;

        public static EventBus<BaseEvent> NetworkEvents { get; private set; } = new EventBus<BaseEvent>();
        public static EventBus<GameEvent> GlobalGameEvents { get; private set; } = new EventBus<GameEvent>();

        private GameWorld _world;

        public void ReceiveInput(PlayerEntity sender, byte[] input)
        {
            BaseEvent ev = Serialization.ToEventRaw(input);
            ev.Sender = sender;
            NetworkEvents.Call(ev);
            DeltaTracker.SendDeltaPackets(sender);
        }

        public GameWorld World
        {
            get => _world; set
            {
                _world = value;
                if (value != null)
                {
                    value.Game = this;
                }
            }
        }

        public StrategyGame(in GameSpec specs, GameWorld world)
        {
            World = world;
            Specs = specs;

        }

        public void ClearEventListeners()
        {
            NetworkEvents.Clear();
        }
    }
}
