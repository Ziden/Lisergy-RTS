using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameData.Specs;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct DungeonSpecId
{
	public byte Id;

	public static implicit operator byte(DungeonSpecId d)
	{
		return d.Id;
	}

	public static implicit operator DungeonSpecId(byte b)
	{
		return new DungeonSpecId {Id = b};
	}

	public override string ToString()
	{
		return Id.ToString();
	}

	public DungeonSpecId(byte id)
	{
		Id = id;
	}
}

[Serializable]
public class DungeonSpec
{
	public ArtSpec Art;
	public List<BattleSpec> BattleSpecs;
	public ushort LootSpecID;
	public DungeonSpecId SpecId;
}