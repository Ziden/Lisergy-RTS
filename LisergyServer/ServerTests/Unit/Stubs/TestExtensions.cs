using Game.Engine.ECLS;
using Game.Engine.Scheduler;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Tile;

namespace Tests.Unit.Stubs
{
    public static class TestExtensions
    {
        public static TileModel GetTile(this IEntity e)
        {
            if (!e.Components.Has<MapPlacementComponent>()) return null;

            var pos = e.Get<MapPlacementComponent>().Position;
            return e.Game.World.GetTile(pos.X, pos.Y);
        }

        public static GameTask Course(this IEntity e)
        {
            var course = e.Get<MovementComponent>().CourseId;
            return e.Game.Scheduler.GetTask(course);
        }

        public static int GetLineOfSight(this IEntity e)
        {
            return e.Get<EntityVisionComponent>().LineOfSight;
        }
    }
}
