using System;
using Game.ECS;
using Game.Systems.Map;
using Game.Tile;
using Game.World;

namespace Game.Systems.Resources
{
	/// <summary>
	/// Logic for any entity that has a cargo component meaning he can harvest resources
	/// </summary>
	public unsafe class CargoLogic : BaseEntityLogic<CargoComponent>
	{
		public void A()
		{
			Entity.Get<CargoComponent>();
		}
	}
}