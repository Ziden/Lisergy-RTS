using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;

namespace Game.Entity
{
    [Serializable]
    public abstract class ExploringEntity : WorldEntity
    {
        public ExploringEntity(PlayerEntity owner) : base(owner) { }

        public abstract byte GetLineOfSight();

        public override Tile Tile
        {
            get { return base.Tile; }
            set
            {
                var los = GetLineOfSight();
                var previousTile = base.Tile;
                HashSet<WorldEntity> oldViewers = null;
                if (previousTile != null)
                {
                    oldViewers = previousTile.Viewing;
                    foreach (var tile in previousTile.GetAOE(los))
                        tile.SetUnseenBy(this);
                }

                Log.Debug($"Entity {this} exploring {los}x{los} on {value}");

                foreach (var tile in value.GetAOE(los))
                    tile.SetSeenBy(this);

                base.Tile = value;
                SendVisibilityPackets(value, previousTile);
            }
        }

        protected virtual void SendVisibilityPackets(Tile newTile, Tile previousTile)
        {
            HashSet<WorldEntity> oldViewers = null;
            if (previousTile != null)
                oldViewers = previousTile.Viewing;

            var newViewers = new HashSet<WorldEntity>(newTile.Viewing);
            if (oldViewers != null)
                newViewers.ExceptWith(oldViewers);

            foreach (var viewer in newTile.Viewing)
                if (newViewers.Remove(viewer))
                    viewer.Owner.Send(new EntityVisibleEvent(this));
        }
    }
}
