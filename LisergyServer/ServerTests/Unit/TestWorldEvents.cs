using Game.World;
using NUnit.Framework;
using ServerTests;

namespace GameUnitTests
{
    public class TestChunk
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [Test]
        public void GetGetTileCoords()
        {
            // get a TileEntity from a chunk in the middle
            var chunkX = 1;
            var tileX = 2;
            var tile = Game.World.GetTile(chunkX * GameWorld.CHUNK_SIZE + tileX, chunkX * GameWorld.CHUNK_SIZE + tileX);

            Assert.AreEqual(chunkX, tile.Chunk.X);
            Assert.AreEqual(chunkX, tile.Chunk.Y);

            Assert.AreEqual(chunkX * GameWorld.CHUNK_SIZE + tileX, (object)tile.X);
            Assert.AreEqual(chunkX * GameWorld.CHUNK_SIZE + tileX, (object)tile.Y);
        }


        [Test]
        public void TestChunkData()
        {
            var chunk = Game.World.GetChunk(1, 1);

            Assert.AreEqual(1, chunk.X);
            Assert.AreEqual(1, chunk.Y);
        }


        [Test]
        public void TestGetStartingChunk()
        {
            var newbieChunk = Game.World.GetUnusedStartingTile().Chunk;

            Assert.IsFalse(newbieChunk == null);
        }
    }
}