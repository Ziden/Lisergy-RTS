using Game;
using Game.Generator;
using Game.World;

namespace GameDataTest.TestWorldGenerator
{
    public class TestWorld : GameWorld
    {
        public TestWorld(IGame game) : base(game, 20, 20)
        {
            Seed = 12345;
            ChunkPopulators.Add(new DungeonsPopulator());
            ChunkPopulators.Add(new NewbieChunkPopulator());
        }
    }
}
