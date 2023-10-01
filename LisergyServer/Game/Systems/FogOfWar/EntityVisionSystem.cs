using Game.ECS;
using Game.Events.GameEvents;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    public class EntityVisionSystem : GameSystem<EntityVisionComponent, WorldEntity>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<EntityMoveInEvent>(OnEntityStepOnTile);
        }

        private static void OnEntityStepOnTile(WorldEntity e, EntityVisionComponent c, EntityMoveInEvent ev)
        {
            UpdateVisionRange(e, ev.FromTile, ev.ToTile);
        }

        private static void UpdateVisionRange(WorldEntity explorer, TileEntity from, TileEntity to)
        {
            var los = explorer.Components.Get<EntityVisionComponent>().LineOfSight;
            if (los > 0)
            {
                HashSet<TileEntity> oldLos = new HashSet<TileEntity>();
                if (from != null)
                    oldLos.UnionWith(from.GetAOE(los));

                HashSet<TileEntity> newLos = new HashSet<TileEntity>();
                if (to != null)
                    newLos.UnionWith(to.GetAOE(los));

                var visEnabled = new TileExplorationStateChanged()
                {
                    Explorer = explorer,
                    Explored = true
                };

                foreach (var tile in newLos.Except(oldLos))
                {
                    visEnabled.Tile = tile;
                    tile.Components.CallEvent(visEnabled);
                }

                visEnabled.Explored = false;
                foreach (var tile in oldLos.Except(newLos).ToList())
                {
                    visEnabled.Tile = tile;
                    tile.Components.CallEvent(visEnabled);
                }
            }
        }

    }
}
