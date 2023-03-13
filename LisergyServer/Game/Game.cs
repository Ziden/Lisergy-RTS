
using Game.Events.Bus;
using GameData;
 

namespace Game
{
    public class StrategyGame
    { 
        public static GameSpec Specs { get; private set; }

        public EventBus NetworkEvents { get; private set; }
        public EventBus GameEvents { get; private set; }

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
            NetworkEvents = new EventBus();
            GameEvents = new EventBus();
        }

        public void ClearEventListeners()
        {
            NetworkEvents.Clear();
        }

        public virtual void GenerateMap()
        {
            
        }
    }
}
