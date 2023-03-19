
using Game.ECS;
using Game.Entity.Components;
using Game.Events.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.World.Systems
{
    public class EntityVisionSystem : GameSystem<EntityVisionComponent , WorldEntity>
    {
        internal override void OnComponentAdded(WorldEntity owner, EntityVisionComponent component, EntityEventBus events)
        {
            events.RegisterComponentEvent<WorldEntity, EntityMoveInEvent, EntityVisionComponent>(OnEntityStepOnTile);
        }

        private static void OnEntityStepOnTile(WorldEntity e, EntityVisionComponent c, EntityMoveInEvent ev)
        {
            UpdateVisionRange(e, ev.FromTile, ev.ToTile);
        }

        private static void UpdateVisionRange(WorldEntity explorer, Tile from, Tile to)
        {
            var los = explorer.Components.Get<EntityVisionComponent>().LineOfSight;
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

    }
}
