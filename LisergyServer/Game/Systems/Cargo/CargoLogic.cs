using Game.Engine.ECLS;

namespace Game.Systems.Resources
{
    /// <summary>
    /// Logic for any entity that has a cargo component meaning he can harvest resources
    /// </summary>
    public unsafe class CargoLogic : BaseEntityLogic<CargoComponent>
    {
        /// <summary>
        /// Checks if there's room on the given unit cargo to store 
        /// Returns the cargo slot available
        /// </summary>
        public int GetAvailableSpace(in ResourceStackData resource)
        {
            var cargo = CurrentEntity.Get<CargoComponent>();
            var slot = cargo.GetRoomFor(resource.ResourceId);
            if (slot == -1) return -1;
            var spec = Game.Specs.Resources[resource.ResourceId];
            var totalWeight = spec.WeightPerUnit * resource.Amount;
            if (totalWeight > cargo.RemainingWeight) return -1;
            return slot;
        }

        /// <summary>
        /// Modifies the given resource stack to have maximum amount that the player can carry
        /// Returns the amount that was trimmed out
        /// </summary>
        public int TrimResourcesToMaxCargo(ref ResourceStackData resource)
        {
            var cargo = CurrentEntity.Get<CargoComponent>();
            var spec = Game.Specs.Resources[resource.ResourceId];
            var canCarry = cargo.RemainingWeight / spec.WeightPerUnit;
            var excess = resource.Amount - canCarry;
            if (excess > 0)
            {
                resource.Amount -= (ushort)excess;
                return excess;
            }
            return 0;
        }

        /// <summary>
        /// Adds the given resource stack to the entity cargo
        /// </summary>
        public bool AddTocargo(in ResourceStackData resource)
        {
            var slot = GetAvailableSpace(resource);
            if (slot == -1)
            {
                Game.Log.Error($"Cargo capacity rached for {resource} to cargo {CurrentEntity.Get<CargoComponent>()} from {CurrentEntity}");
                return false;
            }
            var spec = Game.Specs.Resources[resource.ResourceId];
            var totalWeight = (ushort)(spec.WeightPerUnit * resource.Amount);
            var cargo = CurrentEntity.Components.Get<CargoComponent>();
            
            // Update the appropriate slot directly
            if (slot == 0)
            {
                if (cargo.Slot1.Empty)
                {
                    cargo.Slot1 = new ResourceStackData(resource.ResourceId, resource.Amount);
                }
                else
                {
                    var updatedSlot = cargo.Slot1;
                    updatedSlot.Amount += resource.Amount;
                    cargo.Slot1 = updatedSlot;
                }
            }
            else if (slot == 1)
            {
                if (cargo.Slot2.Empty)
                {
                    cargo.Slot2 = new ResourceStackData(resource.ResourceId, resource.Amount);
                }
                else
                {
                    var updatedSlot = cargo.Slot2;
                    updatedSlot.Amount += resource.Amount;
                    cargo.Slot2 = updatedSlot;
                }
            }
            else if (slot == 2)
            {
                if (cargo.Slot3.Empty)
                {
                    cargo.Slot3 = new ResourceStackData(resource.ResourceId, resource.Amount);
                }
                else
                {
                    var updatedSlot = cargo.Slot3;
                    updatedSlot.Amount += resource.Amount;
                    cargo.Slot3 = updatedSlot;
                }
            }
            
            cargo.CurrentWeight += totalWeight;
            CurrentEntity.Save(cargo);
            Game.Log.Debug($"Added {resource} to cargo {cargo} from {CurrentEntity}");
            return true;
        }
    }
}