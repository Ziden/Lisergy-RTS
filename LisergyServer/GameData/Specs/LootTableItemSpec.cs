using System;
using System.Collections.Generic;

namespace GameData.Specs;

[Serializable]
public class LootTableItemSpec
{
	public double Chance;
	public byte Group;
	public ushort ItemSpecID;
}

[Serializable]
public class LootSpec
{
	public List<LootTableItemSpec> LootTables;
	public ushort SpecID;

	public LootSpec(ushort id)
	{
		SpecID = id;
		LootTables = new List<LootTableItemSpec>();
	}
}