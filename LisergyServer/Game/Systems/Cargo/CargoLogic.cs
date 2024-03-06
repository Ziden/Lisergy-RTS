using Game.Engine.ECS;

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
			var cargo = Entity.Get<CargoComponent>();
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
            var cargo = Entity.Get<CargoComponent>();
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
			if(slot == -1)
			{
				Game.Log.Error($"Cargo capacity rached for {resource} to cargo {Entity.Get<CargoComponent>()} from {Entity}");
				return false;
			}
            var spec = Game.Specs.Resources[resource.ResourceId];
			var totalWeight = (ushort)(spec.WeightPerUnit * resource.Amount);
            var cargo = Entity.Components.GetPointer<CargoComponent>();
			cargo->Add(resource);
			cargo->CurrentWeight += totalWeight;
            Game.Log.Debug($"Added {resource} to cargo {*cargo} from {Entity}");
			return true;
		}
	}
}