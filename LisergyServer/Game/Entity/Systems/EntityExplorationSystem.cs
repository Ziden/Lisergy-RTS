
using Game.ECS;
using Game.Entity.Components;
using Game.Events.GameEvents;
using System.Collections.Generic;
using System.Linq;

namespace Game.World.Systems
{
    public class EntityExplorationSystem : GameSystem<EntityExplorationComponent , WorldEntity>
    {
        internal override void OnComponentAdded(WorldEntity owner, EntityExplorationComponent component, EntitySharedEventBus<WorldEntity> events)
        {
            events.RegisterComponentEvent<EntityMoveInEvent, EntityExplorationComponent>(OnEntityStepOnTile);
        }

        private static void UpdateLineOfSight(WorldEntity explorer, Tile from, Tile to)
        {
            
            var los = explorer.Components.Get<EntityExplorationComponent>().LineOfSight;
            Log.Debug($"Updating LOS of {explorer} (LOS={los})");
            if (los > 0)
            {
                HashSet<Tile> oldLos = new HashSet<Tile>();
                if (from != null)
                    oldLos.UnionWith(from.GetAOE(los));

                HashSet<Tile> newLos = new HashSet<Tile>();
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

        private static void OnEntityStepOnTile(WorldEntity e, EntityExplorationComponent c, EntityMoveInEvent ev)
        {
            UpdateLineOfSight(e, ev.FromTile, ev.ToTile);
        }
    }
}
