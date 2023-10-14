using Game;
using Game.Generator;
using Game.World;

namespace GameDataTest.TestWorldGenerator
{
    public class TestWorld : GameWorld
    {
        public const int MAX_PLAYERS = 40;
        public const int SEED = 12345;

        public override IGame Game
        {
            get => base.Game;
            set
            {
                base.Game = value;
                Populate(SEED,
                    new NewbieChunkPopulator(),
                    new DungeonsPopulator()
                );
            }
        }

        public TestWorld() : base(10, 100, 100)
        {
         
        }
    }
}
