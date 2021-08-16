using Game.Events;
using Game.Movement;
using System;
using System.Collections.Generic;

namespace Game.Entity
{
    [Serializable]
    public abstract class MovableWorldEntity : ExploringEntity
    {
        public MovableWorldEntity(PlayerEntity owner) : base(owner) { }

        public abstract TimeSpan GetMoveDelay();

        [NonSerialized]
        private CourseTask _course;

        public CourseTask Course
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

                // changed tile
                var allViewers = new HashSet<PlayerEntity>();
                if (base.Tile != value && base.Tile != null)
                {
                    allViewers.UnionWith(base.Tile.PlayersViewing);
                    if(value != null)
                        allViewers.UnionWith(value.PlayersViewing);
                }
                    foreach (var viewer in allViewers)
                        viewer.Send(new EntityMoveEvent(this, value));
                
                value.MovingEntities.Add(this);
                base.Tile = value;
            }
        }
    }
}
