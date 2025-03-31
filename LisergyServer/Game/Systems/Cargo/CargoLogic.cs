using Game.Engine.ECLS;
using GameData;
using System.Collections.Generic;

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
        public bool HasRoomFor(in ResourceStackData resource)
        {
            var cargo = CurrentEntity.Get<CargoComponent>();
            if (cargo.Items == null) return true;
            if (!cargo.Items.ContainsKey(resource.ResourceId) && cargo.Items.Count >= cargo.MaxItems) return false;
            var spec = Game.Specs.Resources[resource.ResourceId];
            var totalWeight = spec.WeightPerUnit * resource.Amount;
            if (totalWeight > cargo.RemainingWeight) return false;
            return true;
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

        public ushort GetAmount(ResourceSpecId id)
        {
            var cargo = CurrentEntity.Get<CargoComponent>();
            if (cargo.Items.TryGetValue(id, out var amount))
            {
                return amount;
            }
            return 0;
        }

        /// <summary>
        /// Adds the given resource stack to the entity cargo
        /// </summary>
        public bool AddTocargo(in ResourceStackData resource)
        {
            if (!HasRoomFor(resource))
            {
                Game.Log.Error($"Cargo capacity rached for {resource} to cargo {CurrentEntity.Get<CargoComponent>()} from {CurrentEntity}");
                return false;
            }
            var spec = Game.Specs.Resources[resource.ResourceId];
            var totalWeight = (ushort)(spec.WeightPerUnit * resource.Amount);
            var cargo = CurrentEntity.Components.Get<CargoComponent>();

            if(cargo.Items == null)
            {
                cargo.Items = new Dictionary<ResourceSpecId, ushort>();
            }
            if (cargo.Items.TryGetValue(resource.ResourceId, out var amt))
            {
                cargo.Items[resource.ResourceId] = (ushort)(amt + resource.Amount);
            }
            else
            {
                cargo.Items.Add(resource.ResourceId, resource.Amount);
            }
            cargo.CurrentWeight += totalWeight;
            CurrentEntity.Save(cargo);
            Game.Log.Debug($"Added {resource} to cargo {cargo} from {CurrentEntity}");
            return true;
        }
    }
}