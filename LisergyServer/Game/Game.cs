using Game.Debug;
using Game.Generator;
using GameData;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void LoadMap()
        {
            var worldGen = new Worldgen(World);
            worldGen.Populators.Add(new NewbieChunkPopulator());
            worldGen.Generate(Config.WorldMaxPlayers);
            //MapDebug.PrintAscii(World);
        }
    }
}
