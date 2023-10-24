using System.Runtime.CompilerServices;

namespace Game.World
{
    public abstract class ChunkPopulator
    {
       
        public abstract void Populate(GameWorld w, PreAllocatedChunkMap map, Chunk c);
    }
}
