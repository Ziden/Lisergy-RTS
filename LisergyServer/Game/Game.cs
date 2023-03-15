
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
        public static GameSpec Specs { get; private set; }

        public EventBus<BaseEvent> NetworkEvents { get; private set; }
        public EventBus<GameEvent> GameEvents { get; private set; }

        public List<IGameSystem> Systems { get; private set; }

        private GameWorld _world;
        public GameWorld World { get => _world; set { 
                _world = value;
                if(value != null)
                    value.Game = this;
            } 
        }
        
        public GameSpec GameSpec => Specs;

        public StrategyGame(GameSpec specs, GameWorld world)
        {
            World = world;
            Specs = specs;
            NetworkEvents = new EventBus<BaseEvent>();
            GameEvents = new EventBus<GameEvent>();
        }

        public void ClearEventListeners()
        {
            NetworkEvents.Clear();
        }
    }
}
