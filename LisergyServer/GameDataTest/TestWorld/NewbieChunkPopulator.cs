using Game.World;

namespace Game.Generator
{
    public class NewbieChunkPopulator : ChunkPopulator
    {
        int tries = GameWorld.PLAYERS_CHUNKS;
        int wait = 0;

        public override void Populate(GameWorld w, Chunk c)
        {
            w.Map.SetFlag(c.X, c.Y, ChunkFlag.NEWBIE_CHUNK);

            for (var i = 0; i < GameWorld.CHUNK_SIZE; i++)
            {
                c.GetTile(0, i).SpecId = 1;
                c.GetTile(i, 0).SpecId = 2;
            }

            if (ShouldPopulate(c))
            {
                for (var i = 0; i < GameWorld.CHUNK_SIZE; i++)
                {
                    c.GetTile(i, i).SpecId = 3;
                }

                return;
            }
        }

        public bool ShouldPopulate(Chunk c)
        {
            wait--;
            if (wait > 0)
            {
                return false;
            }
            var rnd = WorldUtils.Random.Next(tries);
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
