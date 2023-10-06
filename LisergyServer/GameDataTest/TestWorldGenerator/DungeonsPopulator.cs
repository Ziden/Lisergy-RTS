
using Game.Systems.Dungeon;
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
                var dungeon = w.Game.Entities.CreateEntity<DungeonEntity>(null);
                var spec = w.Game.Specs.Dungeons[TestDungeons.EASY.DungeonSpecID];
                dungeon.BuildFromSpec(spec);
                w.Game.Systems.Map.GetEntityLogic(dungeon).SetPosition(place);
            }
        }
    }
}
