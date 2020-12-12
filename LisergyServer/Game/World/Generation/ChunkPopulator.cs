using System;

namespace Game.Generator
{
    public abstract class ChunkPopulator
    {
        public abstract void Populate(GameWorld w, Chunk c);
    }
}
