using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game.ECS;
using Game.Systems.Battler;

namespace Game.Systems.Resources
{
	/// <summary>
	/// Represents a resource in cargo
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct CargoResource
	{
		public byte ResourceSpecId;
		public ushort Amount;

		public bool Empty => Amount == 0;
	}
	
	/// <summary>
	/// Component for entities that holds a cargo
	/// Which is an inventory that can hold items.
	/// Can only hold 3 types of resource.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[SyncedComponent]
	public struct CargoComponent : IComponent,  IEnumerable<CargoResource>
	{
		public ushort CurrentWeight;
		public ushort MaxWeight;
		public CargoResource Slot1;
		public CargoResource Slot2;
		public CargoResource Slot3;

		public bool HasRoom => Slot1.Empty || Slot2.Empty || Slot3.Empty;
		public ushort RemainingWeight => (ushort)(MaxWeight - CurrentWeight);

		public CargoResource this[in int i]
		{
			get
			{
				if (i == 0) return Slot1;
				else if (i == 1) return Slot2;
				else if (i == 2) return Slot3;
				throw new ArgumentOutOfRangeException();
			}
		}

		public IEnumerator<CargoResource> GetEnumerator()
		{
			yield return Slot1;
			yield return Slot2;
			yield return Slot3;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}