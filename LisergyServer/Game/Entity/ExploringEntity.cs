using Game.Events.GameEvents;
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
                var t = this.GetType();
                var los = GetLineOfSight();
                if(los > 0)
                {
                    HashSet<Tile> oldLos = new HashSet<Tile>();
                    if (base.Tile != null)
                        oldLos.UnionWith(base.Tile.GetAOE(los));

                    HashSet<Tile> newLos = new HashSet<Tile>();
                    if (value != null)
                        newLos.UnionWith(value.GetAOE(los).ToList());

                    var visEnabled = new TileVisibilityChangedEvent()
                    {
                        Explorer = this,
                        Visible = true
                    };

                    foreach (var tile in newLos.Except(oldLos))
                    {
                        visEnabled.Tile = tile;
                        tile.CallComponentEvents(visEnabled);
                    }
                       

                    visEnabled.Visible = false;
                    foreach (var tile in oldLos.Except(newLos))
                    {
                        visEnabled.Tile = tile;
                        tile.CallComponentEvents(visEnabled);
                    }
                        
                }
                base.Tile = value;
            }
        }
    }
}
