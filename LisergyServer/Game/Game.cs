
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.World.Systems;
using GameData;
using System.Collections.Generic;

namespace Game
{
    public class StrategyGame
    {

        private static GameSpec _spec;
        public static ref GameSpec Specs => ref _spec;

        public ref GameSpec GameSpec => ref _spec;

        public EventBus<BaseEvent> NetworkEvents { get; private set; }
        public static EventBus<GameEvent> GlobalGameEvents { get; private set; }

        public List<IGameSystem> Systems { get; private set; }

        private GameWorld _world;

        public void ReceiveInput(PlayerEntity sender, byte[] input)
        {
            var ev = Serialization.ToEventRaw(input);
            ev.Sender = sender;
            NetworkEvents.Call(ev);
            DeltaTracker.SendDeltaPackets(sender);
        }

        public GameWorld World { get => _world; set { 
                _world = value;
                if(value != null)
                    value.Game = this;
            } 
        } 

        public StrategyGame(in GameSpec specs, GameWorld world)
        {
            World = world;
            Specs = specs;
            NetworkEvents = new EventBus<BaseEvent>();
            GlobalGameEvents = new EventBus<GameEvent>();
        }

        public void ClearEventListeners()
        {
            NetworkEvents.Clear();
        }
    }
}
