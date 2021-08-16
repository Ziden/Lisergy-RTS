using Game.Events.ServerEvents;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

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
                if(los > 0)
                {
                    var previousTile = base.Tile;
                    HashSet<WorldEntity> oldViewers = null;
                    if (previousTile != null)
                    {
                        oldViewers = previousTile.EntitiesViewing;
                        foreach (var tile in previousTile.GetAOE(los))
                            tile.SetUnseenBy(this);
                    }

                    Log.Debug($"Entity {this} exploring {los}x{los} on {value}");
                    foreach (var tile in value.GetAOE(los))
                        tile.SetSeenBy(this);

                    if (value != previousTile)
                        SendEntityVisibilityPackets(value, previousTile);
                }
                base.Tile = value;
            }
        }

        protected virtual void SendEntityVisibilityPackets(Tile newTile, Tile previousTile)
        {
            HashSet<WorldEntity> oldViewers = null;
            if (previousTile != null)
                oldViewers = previousTile.EntitiesViewing;

            var newViewers = new HashSet<WorldEntity>(newTile.EntitiesViewing);
            if (oldViewers != null)
                newViewers.ExceptWith(oldViewers);

            HashSet<PlayerEntity> playerViewers = new HashSet<PlayerEntity>(newViewers.Select(v => v.Owner));
            var ev = new EntityVisibleEvent(this);
            foreach (var viewer in newTile.EntitiesViewing)
                if (playerViewers.Remove(viewer.Owner))
                    viewer.Owner.Send(ev);   
        }
    }
}
