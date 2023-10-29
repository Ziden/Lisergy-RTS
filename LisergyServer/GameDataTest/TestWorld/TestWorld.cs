using Game;
using Game.Generator;
using Game.World;

namespace GameDataTest.TestWorldGenerator
{
    public class TestWorld : GameWorld
    {
        public const int MAX_PLAYERS = 40;
        public const int SEED = 12345;

        public TestWorld(IGame game) : base(game, 100, 100)
        {
           Populate(SEED,
               new NewbieChunkPopulator(),
               new DungeonsPopulator()
           );
        }
    }
}
