using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Generator
{
    public static class TestWorldGenerator
    {
        public static (int, int) MeasureWorld(int playerCount)
        {
            var amtOfChunks = playerCount * GameWorld.PLAYERS_CHUNKS;
            var amtOfTiles = amtOfChunks * GameWorld.TILES_IN_CHUNK;
            var arraySize = (int)Math.Ceiling(Math.Sqrt(amtOfTiles));
            var extraNeeded = GameWorld.CHUNK_SIZE - arraySize % GameWorld.CHUNK_SIZE;
            var SizeX = arraySize + extraNeeded;
            var SizeY = arraySize + extraNeeded;
            return (SizeX, SizeY);
        }

        public static GameWorld PopulateWorld(GameWorld world, int seed = 0, params ChunkPopulator[] populators)
        {
           
       
            if (seed == 0)
                seed = new Random().Next(0, ushort.MaxValue);
            WorldUtils.SetRandomSeed(seed);
            world.Seed = (ushort)seed;

            Log.Info("Generated Seed " + seed);
            Log.Debug($"Generating world {world.SizeX}x{world.SizeY} for {world.Players.MaxPlayers} players");
            foreach (var chunk in world.Map.AllChunks())
                foreach (var populator in populators)
                    populator.Populate(world, chunk);

            return world;
        }

    }
}
