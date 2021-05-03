using Game.Events;
using Game.Movement;
using System;

namespace Game.Entity
{
    [Serializable]
    public abstract class MovableWorldEntity : ExploringEntity
    {
        public MovableWorldEntity(PlayerEntity owner) : base(owner) { }

        public abstract TimeSpan GetMoveDelay();

        [NonSerialized]
        private MovementTask _course;

        public MovementTask Course
        {
            get => _course;
            set
            {
                if (_course != null && !_course.HasFinished)
                    _course.Cancel();
                _course = value;
            }
        }

        public override Tile Tile
        {
            get { return base.Tile; }
            set
            {
                if (base.Tile != null)
                    base.Tile.MovingEntities.Remove(this);

                if (base.Tile != value && base.Tile != null && value != null)
                    foreach (var viewer in value.PlayersViewing)
                        viewer.Send(new EntityMoveEvent(this, value));

                base.Tile = value;
                base.Tile.MovingEntities.Add(this);
            }
        }
    }
}
