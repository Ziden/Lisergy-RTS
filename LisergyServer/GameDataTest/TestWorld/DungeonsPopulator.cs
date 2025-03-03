using Game.Entities;
using Game.World;
using GameDataTest;

namespace Game.Generator
{
    public class DungeonsPopulator : ChunkPopulator
    {
        public override void Populate(GameWorld w, ServerChunkMap map, Chunk c)
        {
            var place = c.FindTileWithId(0);
            if (place != null)
            {
                var dungeon = w.Game.Entities.CreateEntity(EntityType.Dungeon);
                var spec = w.Game.Specs.Dungeons[TestDungeons.EASY.SpecId];
                dungeon.Logic.Dungeon.SetUnitsFromSpec(spec);
                dungeon.Logic.Map.SetPosition(place);
            }
        }
    }
}
