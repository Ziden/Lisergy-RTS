using Game.World;

namespace ClientSDK.Data
{
    /// <summary>
    /// Chunk map that can lazy load tiles and chunk into memory
    /// </summary>
    public class LazyLoadChunkMap : ChunkMap
    {
        public LazyLoadChunkMap(GameWorld world, int tilesAmtX, int tilesAmtY) : base(world, tilesAmtX, tilesAmtY)
        {
        }
    }
}
