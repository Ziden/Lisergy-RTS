using Game.Events.GameEvents;
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
                    HashSet<Tile> oldLos = new HashSet<Tile>();
                    if (base.Tile != null)
                        oldLos.UnionWith(base.Tile.GetAOE(los));

                    HashSet<Tile> newLos = new HashSet<Tile>();
                    if (value != null)
                        newLos.UnionWith(value.GetAOE(los).ToList());

                    foreach (var tile in newLos.Except(oldLos))
                        tile.SetSeenBy(this);

                    foreach (var tile in oldLos.Except(newLos))
                        tile.SetUnseenBy(this);
                }
                base.Tile = value;
            }
        }

       
    }
}
