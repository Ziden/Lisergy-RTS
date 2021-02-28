using Game.Generator;
using Game.Scheduler;
using GameData;

namespace Game
{
    public class StrategyGame
    {
        public static GameSpec Specs { get; private set; }
        public static GameConfiguration Config { get; private set; }

        public GameScheduler Scheduler;
        public GameWorld World { get; private set; }
     
        public StrategyGame(GameConfiguration cfg, GameSpec specs, GameWorld world)
        {
            World = world;
            Specs = specs;
            Config = cfg;
            Scheduler = new GameScheduler();
        }

        public virtual void GenerateMap()
        {
            var worldGen = new Worldgen(World);
            worldGen.Populators.Add(new NewbieChunkPopulator());
            worldGen.Generate(Config.WorldMaxPlayers);
        }
    }
}
