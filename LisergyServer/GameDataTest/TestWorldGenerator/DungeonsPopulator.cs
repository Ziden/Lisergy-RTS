using Game.Dungeon;
using Game.World;
using GameDataTest;

namespace Game.Generator
{
    public class DungeonsPopulator : ChunkPopulator
    {
        public override void Populate(GameWorld w, Chunk c)
        {
            var place = c.FindTileWithId(0);
            if (place != null)
            {
                var dungeon = new DungeonEntity(TestDungeons.EASY.DungeonSpecID);
                dungeon.Tile = place;
            }
        }
    }
}
