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
        public static GameConfiguration Config { get; private set; }

        public GameWorld World { get; private set; }
        public List<EventListener> _listeners = new List<EventListener>();

        public StrategyGame(GameConfiguration cfg, GameSpec specs, GameWorld world)
        {
            World = world;
            Specs = specs;
            Config = cfg;
        }

        public void RegisterEventListeners()
        {
            var networkEvents = new NetworkEvents();
            
            _listeners.Add(new WorldListener(World));
            _listeners.Add(new CourseListener(World));
           
        }

        public void ClearEventListeners()
        {
            foreach (var listener in _listeners)
                listener.Dispose();
            _listeners.Clear();
        }

        public virtual void GenerateMap()
        {
            var worldGen = new Worldgen(World);
            worldGen.Populators.Add(new NewbieChunkPopulator());
            worldGen.Generate(Config.WorldMaxPlayers);
        }
    }
}
