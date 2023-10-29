using Game.ECS;

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
			var slot = cargo.GetRoomFor(resource.SpecId);
            if (slot == -1) return -1;
			var spec = Game.Specs.Resources[resource.SpecId];
			var totalWeight = spec.WeightPerUnit * resource.Amount;
			if (totalWeight > cargo.RemainingWeight) return -1;
			return slot;
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
            var spec = Game.Specs.Resources[resource.SpecId];
			var totalWeight = (ushort)(spec.WeightPerUnit * resource.Amount);
            var cargo = Entity.Components.GetPointer<CargoComponent>();
			cargo->Add(resource);
			cargo->CurrentWeight += totalWeight;
            Game.Log.Debug($"Added {resource} to cargo {*cargo} from {Entity}");
			return true;
		}
	}
}