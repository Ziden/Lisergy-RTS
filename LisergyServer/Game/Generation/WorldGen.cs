using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Generator
{
    public class Worldgen
    {
        public GameWorld world;

        public List<ChunkPopulator> Populators = new List<ChunkPopulator>();

        private int _seed;

        public Worldgen(GameWorld w)
        {
            world = w;
        }

        public virtual void Generate(int qtdPlayers, int seed = 0)
        {
            world.CreateWorld(qtdPlayers);
            GenerateTiles();
            if (seed == 0)
                _seed = new Random().Next(0, ushort.MaxValue);
            else
                _seed = seed;
            WorldUtils.SetRandomSeed(_seed);
            world.Seed = (ushort)_seed;
            Log.Info("Generated Seed " + _seed);
            Log.Debug($"Generating world {world.SizeX}x{world.SizeY} for {world.Players.MaxPlayers} players"); 
            PopulateChunks();
        }

        public virtual Tile GenerateTile(Chunk c, int x, int y)
        {
            return new Tile(c, x, y);
        }

        public virtual void GenerateTiles()
        {
            var maxChunkX = world.SizeX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var maxChunkY = world.SizeY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            for (var chunkX = 0; chunkX < maxChunkX; chunkX++)
            {
                for (var chunkY = 0; chunkY < maxChunkY; chunkY++)
                {
                    var tiles = new Tile[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE];
                    var chunk = new Chunk(world.ChunkMap, chunkX, chunkY, tiles);

                    for (var x = 0; x < GameWorld.CHUNK_SIZE; x++)
                    {
                        for (var y = 0; y < GameWorld.CHUNK_SIZE; y++)
                        {
                            var tileX = chunkX * GameWorld.CHUNK_SIZE + x;
                            var tileY = chunkY * GameWorld.CHUNK_SIZE + y;
                            tiles[x, y] = GenerateTile(chunk, tileX, tileY);
                        }
                    }
                    world.ChunkMap.Add(chunk);
                }
            }
        }

        public virtual void PopulateChunks()
        {
            foreach (var chunk in world.ChunkMap.AllChunks())
                foreach (var populator in Populators)
                    populator.Populate(world, chunk);
        }
    }
}
