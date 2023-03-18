using Game.Events;
using Game.Events.GameEvents;
using Game.Movement;
using Game.World;
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
            get => base.Tile; set
            {
                var oldTile = base.Tile;
                base.Tile = value;

                if (oldTile != null)
                {
                    oldTile.Components.CallEvent(new EntityMoveOutEvent()
                    {
                        Entity = this,
                        ToTile = value,
                        FromTile = oldTile
                    });
                }
                if (value != null)
                {
                    value.Components.CallEvent(new EntityMoveInEvent()
                    {
                        Entity = this,
                        ToTile = _tile,
                        FromTile = oldTile
                    });
                } 

                Log.Info($"Placed {this} in {_tile}");
            }
        }
    }
}
