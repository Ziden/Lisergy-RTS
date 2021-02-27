using Game.Generator;
using GameData;

namespace Game
{
    public class StrategyGame
    {
        public GameWorld World { get; private set; }
        public static GameSpec Specs { get; private set; }
        public static GameConfiguration Config { get; private set; }

        public StrategyGame(GameConfiguration cfg, GameSpec specs, GameWorld world)
        {
            World = world;
            Specs = specs;
            Config = cfg;
        }

        public void Tick()
        {

        }

        public virtual void GenerateMap()
        {
            var worldGen = new Worldgen(World);
            worldGen.Populators.Add(new NewbieChunkPopulator());
            worldGen.Generate(Config.WorldMaxPlayers);
        }
    }
}
