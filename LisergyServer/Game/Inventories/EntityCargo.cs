using Game.Entity;
using System;
using System.Collections.Generic;

namespace Game.Inventories
{
    [Serializable]
    public class EntityCargo : Inventory
    {
        [NonSerialized]
        private WorldEntity _owner;

        public WorldEntity Owner { get => _owner; }

        public EntityCargo(WorldEntity owner)
        {
            this._owner = owner;
        }
    }
}

