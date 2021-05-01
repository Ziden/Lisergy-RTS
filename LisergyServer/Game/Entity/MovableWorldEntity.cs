using Game.Events;
using System;

namespace Game.Entity
{
    [Serializable]
    public abstract class MovableWorldEntity : ExploringEntity
    {
        public MovableWorldEntity(PlayerEntity owner) : base(owner) { }

        public abstract TimeSpan GetMoveDelay();

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
