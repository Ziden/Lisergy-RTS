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

        public StrategyGame(GameConfiguration cfg, GameSpec specs)
        {
            World = new GameWorld(cfg.WorldMaxPlayers);
            Specs = specs;
        }

        public void GenerateMap()
        {
            var worldGen = new Worldgen(World);
            worldGen.Populators.Add(new NewbieChunkPopulator());
            worldGen.Generate();
            MapDebug.PrintAscii(World);
        }
    }
}
