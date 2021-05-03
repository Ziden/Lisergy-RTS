using Game.Events;
using Game.Generator;
using Game.Listeners;
using Game.Scheduler;
using GameData;
using LisergyServer;
using System.Collections.Generic;

namespace Game
{
    public class StrategyGame
    {
        public static GameSpec Specs { get; private set; }

        public GameWorld World { get; set; }
        public List<EventListener> _listeners = new List<EventListener>();

        public GameSpec GameSpec => Specs;

        public StrategyGame(GameSpec specs, GameWorld world)
        {
            World = world;
            Specs = specs;
        }

        public void RegisterEventListeners()
        {
            var networkEvents = new NetworkEvents();
            
            _listeners.Add(new WorldListener(World));
            _listeners.Add(new CourseListener(World));
            _listeners.Add(new BattleListener(World));
        }

        public void ClearEventListeners()
        {
            foreach (var listener in _listeners)
                listener.Dispose();
            _listeners.Clear();
        }

        public virtual void GenerateMap()
        {
            
        }
    }
}
