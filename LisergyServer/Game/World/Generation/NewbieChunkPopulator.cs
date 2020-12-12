using Game.World;
using System;

namespace Game.Generator
{
    public class NewbieChunkPopulator : ChunkPopulator
    {
        int tries = GameWorld.PLAYERS_CHUNKS;
        int wait = 0;

        private static bool DEBUG = false;

        public override void Populate(GameWorld w, Chunk c)
        {
            if (!ShouldPopulate(c))
            {
                Log.Debug($"Skipping {c.ToString()}");
                return;
            }

            Log.Debug($"Populating {c.ToString()}");
            w.ChunkMap.SetFlag(c.X, c.Y, ChunkFlag.NEWBIE_CHUNK);

            var bushes = GameWorld.CHUNK_SIZE;
            AddRandomTerrain(w, c, bushes, TerrainData.BUSHES);

            var forests = GameWorld.CHUNK_SIZE;
            AddRandomTerrain(w, c, forests, TerrainData.FOREST);

            var mountains = GameWorld.CHUNK_SIZE / 2;
            AddRandomTerrain(w, c, mountains, TerrainData.MOUNTAIN, TerrainData.FOREST);

            var hills = GameWorld.CHUNK_SIZE;
            AddRandomTerrain(w, c, hills, TerrainData.HILL, TerrainData.MOUNTAIN);

            var water = GameWorld.CHUNK_SIZE / 4;
            AddRandomTerrain(w, c, water, TerrainData.WATER, TerrainData.MOUNTAIN, TerrainData.FOREST);
        }

        public void AddRandomTerrain(GameWorld w, Chunk c, int amt, params TerrainData[] not)
        {
            var desiredTerrain = not[0]; 
            for (var i = 0; i < amt; i++)
            {
                var tile = Worldgen.FindTileWithout(c.Tiles, not);
                if (tile != null)
                {
                    var terrainData = tile.TerrainData;
                    terrainData.AddFlag(desiredTerrain);
                    tile.TerrainData = terrainData;
                }
            }
        }

        public bool ShouldPopulate(Chunk c)
        {
            wait--;
            if (wait > 0)
            {
                return false;
            }
            var rnd = Worldgen.rnd.Next(tries);
            if (rnd == 0)
            {
                wait = tries;
                tries = GameWorld.PLAYERS_CHUNKS;
                return true;
            }
            else
            {
                tries--;
                return false;
            }
        }
    }
}
