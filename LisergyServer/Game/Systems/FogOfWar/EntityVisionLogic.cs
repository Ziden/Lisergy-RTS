using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    public unsafe class EntityVisionLogic : BaseEntityLogic<EntityVisionComponent>
    {
        /// <summary>
        /// Line of sight hashsets cached so no need to keep instantiating
        /// </summary>
        private HashSet<TileEntity> _oldLineOfSight = new HashSet<TileEntity>();
        private HashSet<TileEntity> _newLineOfSight = new HashSet<TileEntity>();

        /// <summary>
        /// Calculates an entity line of sight based on the entity group component.
        /// The entity line of sight is the max group line of sight
        /// </summary>
        public void UpdateGroupLineOfSight()
        {
            var group = Entity.Get<BattleGroupComponent>();
            if (group.Units.Empty)
            {
                Entity.Components.GetPointer<EntityVisionComponent>()->LineOfSight = 0;
            }
            else
            {
                var lineOfSight = group.Units.Max(u => Game.Specs.Units[u.SpecId].LOS);
                Entity.Components.GetPointer<EntityVisionComponent>()->LineOfSight = lineOfSight;
            }
        }

        /// <summary>
        /// Updates the tile visibility of nearby tiles according to an entity movement.
        /// Will unsee some tiles and see new tiles depending the from/to the entity moved
        /// </summary>
        public void UpdateVisionRange(IEntity explorer, TileEntity from, TileEntity to)
        {
            var los = explorer.Components.Get<EntityVisionComponent>().LineOfSight;
            if (los > 0)
            {
                _oldLineOfSight.Clear();
                _newLineOfSight.Clear();

                if (from != null)
                    _oldLineOfSight.UnionWith(from.GetAOE(los));

           
                if (to != null)
                    _newLineOfSight.UnionWith(to.GetAOE(los));

                var visEnabled = new EntityTileVisibilityUpdateEvent()
                {
                    Explorer = explorer,
                    Explored = true
                };

                foreach (var newTileSeen in _newLineOfSight.Except(_oldLineOfSight))
                {
                    visEnabled.Tile = newTileSeen;
                    newTileSeen.Components.CallEvent(visEnabled);
                }

                visEnabled.Explored = false;
                foreach (var oldTileUnseen in _oldLineOfSight.Except(_newLineOfSight))
                {
                    visEnabled.Tile = oldTileUnseen;
                    oldTileUnseen.Components.CallEvent(visEnabled);
                }
            }
        }
    }
}
