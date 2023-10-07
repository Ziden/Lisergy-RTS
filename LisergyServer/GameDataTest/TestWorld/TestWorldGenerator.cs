using Game.World;
using System;

namespace Game.Generator
{
    public static class TestWorldGenerator
    {
        public static (ushort, ushort) MeasureWorld(int playerCount)
        {
            var amtOfChunks = playerCount * GameWorld.PLAYERS_CHUNKS;
            var amtOfTiles = amtOfChunks * GameWorld.TILES_IN_CHUNK;
            var arraySize = (int)Math.Ceiling(Math.Sqrt(amtOfTiles));
            var extraNeeded = GameWorld.CHUNK_SIZE - arraySize % GameWorld.CHUNK_SIZE;
            var SizeX = (ushort)(arraySize + extraNeeded);
            var SizeY = (ushort)(arraySize + extraNeeded);
            return (SizeX, SizeY);
        }
    }
}
