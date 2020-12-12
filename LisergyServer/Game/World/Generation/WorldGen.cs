using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Generator
{
    public class Worldgen
    {
        public static Random rnd;
        public GameWorld world;

        public List<ChunkPopulator> Populators = new List<ChunkPopulator>();

        private int _seed;

        public Worldgen(GameWorld w)
        {
            world = w;
        }

        public void Generate(int seed = 0)
        {
            if (seed == 0)
                _seed = new Random().Next(0, ushort.MaxValue);
            else
                _seed = seed;
            rnd = new Random(_seed);
            world.Seed = (ushort)_seed;
            Log.Info("Generated Seed " + _seed);
            var size = world.GetSize();
            Log.Debug($"Generating world {size}x{size} for {world.Players.MaxPlayers} players");
            GenerateTiles();
            // generateTemperatures();
            // geneerateTerrain();
            PopulateChunks();
        }

        public void GenerateTiles()
        {
            var maxChunkX = world.GetSize() >> GameWorld.CHUNK_SIZE_BITSHIFT;
            for (var chunkX = 0; chunkX < maxChunkX; chunkX++)
            {
                for (var chunkY = 0; chunkY < maxChunkX; chunkY++)
                {
                    var tiles = new Tile[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE];
                    var chunk = new Chunk(world, chunkX, chunkY, tiles);

                    for (var x = 0; x < GameWorld.CHUNK_SIZE; x++)
                    {
                        for (var y = 0; y < GameWorld.CHUNK_SIZE; y++)
                        {
                            var tileX = chunkX * GameWorld.CHUNK_SIZE + x;
                            var tileY = chunkY * GameWorld.CHUNK_SIZE + y;
                            tiles[x, y] = new Tile(chunk, tileX, tileY);
                        }
                    }
                    world.ChunkMap.Add(chunk);
                }
            }
        }


        public static Tile FindTileWithout(Tile[,] tiles, params TerrainData[] data)
        {
            var tries = 10;
            while (tries > 0)
            {
                var rndX = Worldgen.rnd.Next(0, tiles.GetLength(0));
                var rndY = Worldgen.rnd.Next(0, tiles.GetLength(1));
                Tile tile = tiles[rndX, rndY];
                var allGood = true;
                foreach (var terrainData in data)
                {
                    if (tile.TerrainData.HasFlag(terrainData))
                    {
                        allGood = false;
                        break;
                    }
                }
                if (allGood)
                    return tile;
                tries--;
            }
            return null;
        }


        public void PopulateChunks()
        {
            foreach (var chunk in world.ChunkMap.AllChunks())
                foreach (var populator in Populators)
                    populator.Populate(world, chunk);
        }
    }
}
