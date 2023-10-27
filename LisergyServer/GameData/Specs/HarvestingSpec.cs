using System;
using System.Collections.Generic;

namespace GameData.Specs
{
	[Serializable]
	public class HarvestingSpec
	{
		/// <summary>
		/// Default party max cargo
		/// </summary>
		public ushort StartingPartyCargoWeight = 100;

		/// <summary>
		/// When cargo above this percentage number
		/// Unit will move slower
		/// 80 = 80%
		/// </summary>
		public byte CargoFullLimitPct = 80;
		
		/// <summary>
		/// How much of the speed is affected when cargo is full ?
		/// </summary>
		public byte WeightSpeedReduction = 255;

		


	}
}