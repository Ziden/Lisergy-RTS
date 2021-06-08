using BattleServer;
using Game.Events.Bus;
using Game.Listeners;
using Game.World;
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
        }

        public virtual void RegisterEventListeners()
        {
            NetworkEvents = new EventBus();
            GameEvents = new EventBus();
            NetworkEvents.RegisterListener(new BattlePacketListener(World));
            NetworkEvents.RegisterListener(new WorldPacketListener(World));
            NetworkEvents.RegisterListener(new CoursePacketListener(World));
            GameEvents.RegisterListener(new WorldGameListener(this));
        }

        public ListenerType GetListener<ListenerType>() where ListenerType: IEventListener
        {
            return (ListenerType)NetworkEvents.GetListener(typeof(ListenerType));
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
