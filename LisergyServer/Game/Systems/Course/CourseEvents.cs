using Game.ECS;
using Game.Events;
using Game.Systems.Battler;
using Game.Systems.Movement;
using Game.Tile;

namespace Game.Systems.Course
{
    /// <summary>
    /// Whenever a course finishes its last tile movement
    /// </summary>
    public class CourseFinishEvent : IGameEvent
    {
        public IEntity Entity;
        public CourseIntent Intent;
        public TileEntity ToTile;

        public override string ToString() => $"<CourseFinish Entity={Entity} Intent={Intent} Tile={ToTile}/>";
    }

    /// <summary>
    /// Triggered when a course started
    /// </summary>
    public class CourseStartEvent : IGameEvent
    {
        public IEntity Entity;
        public CourseIntent Intent;

        public override string ToString() => $"<CourseStart Entity={Entity} Intent={Intent}/>";
    }

}
