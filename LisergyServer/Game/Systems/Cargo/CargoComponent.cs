using Game.Engine.ECLS;
using GameData;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game.Systems.Resources
{
    /// <summary>
    /// Component for entities that holds a cargo
    /// Which is an inventory that can hold items.
    /// Can only hold 3 types of resource.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [SyncedComponent]
    public class CargoComponent : IComponent
    {
        public Dictionary<ResourceSpecId, ushort> Items;
        public ushort CurrentWeight;
        public ushort MaxWeight;
        public byte MaxItems = 4;

        /// <summary>
        /// Gets the remaining weight to be used on this cargo
        /// </summary>
        public ushort RemainingWeight => (ushort)(MaxWeight - CurrentWeight);

        /// <summary>
        /// Gets current owned amount of a given resource
        /// </summary>
        public ushort GetAmount(in ResourceSpecId id)
        {
            if(Items.TryGetValue(id, out var amount))
            {
                return amount;
            }
            return 0;
        }

        public override string ToString()
        {
            return $"<Cargo Items={Items?.Count ?? 0} Weight={CurrentWeight}/{MaxWeight} >";
        }
    }
}