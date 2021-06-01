using BattleServer;
using Game.Battle;
using Game.Events;
using Game.Events.Bus;
using Game.Generator;
using Game.Listeners;
using Game.Scheduler;
using GameData;
using LisergyServer;
using System.Collections.Generic;
using System.Linq; 

namespace Game
{
    public class StrategyGame
    { 
        public static GameSpec Specs { get; private set; }
        public EventBus NetworkEvents { get; private set; }

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

        public void RegisterEventListeners()
        {
            NetworkEvents = new EventBus();
            NetworkEvents.RegisterListener(new BattleListener(World));
            NetworkEvents.RegisterListener(new WorldListener(World));
            NetworkEvents.RegisterListener(new CourseListener(World));
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
