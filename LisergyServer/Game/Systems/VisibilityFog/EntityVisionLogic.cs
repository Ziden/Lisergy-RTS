using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Entities;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    public unsafe class EntityVisionLogic : BaseEntityLogic<EntityVisionComponent>
    {
        /// <summary>
        /// Line of sight hashsets cached so no need to keep instantiating
        /// </summary>
        private HashSet<TileModel> _oldLineOfSight = new HashSet<TileModel>();
        private HashSet<TileModel> _newLineOfSight = new HashSet<TileModel>();

        public bool IsVisibleFor(IEntity viewer)
        {
            if (viewer.EntityType == EntityType.Player)
            {
                return GetPlayersViewing().Contains(viewer.EntityId);
            }
            else
            {
                return GetEntitiesViewing().Contains(viewer.EntityId);
            }
        }

        /// <summary>
        /// Gets all players viewing the current tile or entity
        /// </summary>
        public IReadOnlyCollection<GameId> GetPlayersViewing()
        {
            var comp = GetVisibility();
            if (comp != null) return comp.PlayersViewing;
            return Array.Empty<GameId>();
        }

        public TileVisibilityComponent GetVisibility()
        {
            if (CurrentEntity.EntityType == EntityType.Tile)
            {
                return CurrentEntity.Components.Get<TileVisibilityComponent>();
            }
            else if (CurrentEntity.Components.TryGet<MapPlacementComponent>(out var c))
            {
                var tile = Game.World.GetTile(c.Position);
                return tile.Components.Get<TileVisibilityComponent>();
            }
            return null;
        }

        /// <summary>
        /// Gets all entities viewing the current tile or entity
        /// </summary>
        public IReadOnlyCollection<GameId> GetEntitiesViewing()
        {
            var comp = GetVisibility();
            if (comp != null) return comp.EntitiesViewing;
            return Array.Empty<GameId>();
        }


        /// <summary>
        /// Calculates an entity line of sight based on the entity group component.
        /// The entity line of sight is the max group line of sight
        /// </summary>
        public void UpdateGroupLineOfSight()
        {
            Console.WriteLine("UpdateGroupLineOfSight " + CurrentEntity);
            var group = CurrentEntity.Get<BattleGroupComponent>();
            if (group.Units.Empty)
            {
                var c = CurrentEntity.Components.Get<EntityVisionComponent>();
                c.LineOfSight = 0;
                CurrentEntity.Save(c);
            }
            else
            {
                var lineOfSight = group.Units.Max(u => Game.Specs.Units[u.SpecId].LOS);
                var c = CurrentEntity.Components.Get<EntityVisionComponent>();
                c.LineOfSight = lineOfSight;
                CurrentEntity.Save(c);
            }
        }

        /// <summary>
        /// Updates the tile visibility of nearby tiles according to an entity movement.
        /// Will unsee some tiles and see new tiles depending the from/to the entity moved
        /// </summary>
        public void UpdateVisionRange(TileModel from, TileModel to)
        {
            Console.WriteLine($"UpdateVisionRange {CurrentEntity} {from} {to}");
            var explorer = CurrentEntity;
            var los = explorer.Components.Get<EntityVisionComponent>().LineOfSight;
            Game.Log.Debug($"Updating entity {explorer} vision range of {los}");
            if (los > 0)
            {
                _oldLineOfSight.Clear();
                _newLineOfSight.Clear();

                if (from != null)
                    _oldLineOfSight.UnionWith(from.GetAOE(los));

                if (to != null)
                    _newLineOfSight.UnionWith(to.GetAOE(los));

                var newExplored = _newLineOfSight.Except(_oldLineOfSight);
                foreach (var newTileSeen in newExplored)
                {
                    OnTileExplorationChanged(newTileSeen, explorer, true);
                }

                var unexplored = _oldLineOfSight.Except(_newLineOfSight);
                foreach (var oldTileUnseen in unexplored)
                {
                    OnTileExplorationChanged(oldTileUnseen, explorer, false);
                }
            }
        }

        private void OnTileExplorationChanged(TileModel tile, IEntity explorer, bool explored)
        {
            var tileObj = tile;
            var tileVisComp = tile.Components.Get<TileVisibilityComponent>() ?? new TileVisibilityComponent();
            var habitants = tile.Components.Get<TileHabitantsComponent>() ?? new TileHabitantsComponent();
            var owner = Game.Entities[explorer.OwnerID];
            if (explored)
            {
                tileVisComp.EntitiesViewing.Add(explorer.EntityId);
                if (owner != null && !tileVisComp.PlayersViewing.Contains(explorer.OwnerID))
                {
                    tileVisComp.PlayersViewing.Add(explorer.OwnerID);
                    var e = EventPool<TileVisibilityChangedEvent>.Get();
                    e.Explorer = explorer;
                    e.Tile = tile;
                    e.Visible = true;
                    owner.Components.CallEvent(e);
                    EventPool<TileVisibilityChangedEvent>.Return(e);
                }
                tile.Save(tileVisComp);
            }
            else
            {
                tileVisComp.EntitiesViewing.Remove(explorer.EntityId);
                if (!tileVisComp.EntitiesViewing.Any(e => Game.Entities[e].OwnerID == explorer.OwnerID))
                {
                    if (owner != null && tileVisComp.PlayersViewing.Remove(owner.EntityId))
                    {
                        var e = EventPool<TileVisibilityChangedEvent>.Get();
                        e.Explorer = explorer;
                        e.Tile = tile;
                        e.Visible = false;
                        owner.Components.CallEvent(e);
                        EventPool<TileVisibilityChangedEvent>.Return(e);
                    }
                }
                tile.Save(tileVisComp);
            }
        }
    }
}
