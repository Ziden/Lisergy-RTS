using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    public class EntityVisionSystem : GameSystem<EntityVisionComponent>
    {
        public EntityVisionSystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<EntityMoveInEvent>(OnEntityStepOnTile);
            EntityEvents.On<UnitAddToGroupEvent>(OnUnitAdded);
            EntityEvents.On<UnitRemovedEvent>(OnUnitRemoved);
        }

        private void OnUnitAdded(IEntity e, ref EntityVisionComponent component, UnitAddToGroupEvent ev)
        {
            component.LineOfSight = ev.Units.Max(u => Game.Specs.Units[u.SpecId].LOS);
            e.Components.Save(component);
        }

        private void OnUnitRemoved(IEntity e, ref EntityVisionComponent component, UnitRemovedEvent ev)
        {
            if (ev.Group.Units.Count == 0) component.LineOfSight = 0;
            else component.LineOfSight = ev.Group.Units.Max(u => Game.Specs.Units[u.SpecId].LOS);
            e.Components.Save(component);
        }

        private void OnEntityStepOnTile(IEntity e, ref EntityVisionComponent c, EntityMoveInEvent ev)
        {
            UpdateVisionRange(e, ev.FromTile, ev.ToTile);
        }

        private void UpdateVisionRange(IEntity explorer, TileEntity from, TileEntity to)
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
