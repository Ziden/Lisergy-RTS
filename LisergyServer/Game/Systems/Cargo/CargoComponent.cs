using Game.Engine.ECLS;
using GameData;
using System;
using System.Collections;
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
    public class CargoComponent : IComponent, IEnumerable<ResourceStackData>
    {
        public ushort CurrentWeight;
        public ushort MaxWeight;
        public ResourceStackData Slot1;
        public ResourceStackData Slot2;
        public ResourceStackData Slot3;

        /// <summary>
        /// Gets the remaining weight to be used on this cargo
        /// </summary>
        public ushort RemainingWeight => (ushort)(MaxWeight - CurrentWeight);

        /// <summary>
        /// Gets a free slot in the cargo for the given resource
        /// </summary>
        public int GetRoomFor(in ResourceSpecId resource)
        {
            if (Slot1.Empty || Slot1.ResourceId == resource) return 0;
            else if (Slot2.Empty || Slot2.ResourceId == resource) return 1;
            else if (Slot3.Empty || Slot3.ResourceId == resource) return 2;
            return -1;
        }

        /// <summary>
        /// Gets current owned amount of a given resource
        /// </summary>
        public ushort GetAmount(in ResourceSpecId id)
        {
            foreach (var r in this) if (r.ResourceId == id) return r.Amount;
            return 0;
        }

        public void Add(in ResourceStackData stack)
        {
            if (Slot1.CanAdd(stack)) Slot1.Add(stack);
            else if (Slot2.CanAdd(stack)) Slot2.Add(stack);
            else if (Slot3.CanAdd(stack)) Slot3.Add(stack);
            else throw new ArgumentOutOfRangeException();
        }

        public void GetStackAtSlot(in int i, out ResourceStackData stack)
        {
            if (i == 0) stack = Slot1;
            else if (i == 1) stack = Slot2;
            else if (i == 2) stack = Slot3;
            throw new ArgumentOutOfRangeException();
        }

        public IEnumerator<ResourceStackData> GetEnumerator()
        {
            yield return Slot1;
            yield return Slot2;
            yield return Slot3;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"<Cargo Weight={CurrentWeight}/{MaxWeight} S1={Slot1} S2={Slot2} S3={Slot3}>";
        }
    }
}