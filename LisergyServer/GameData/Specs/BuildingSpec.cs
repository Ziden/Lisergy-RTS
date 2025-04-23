using System;
using System.Runtime.InteropServices;
using GameData.Specs;

namespace GameData;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct BuildingSpecId
{
	public byte Id;

	public static implicit operator byte(BuildingSpecId d)
	{
		return d.Id;
	}

	public static implicit operator BuildingSpecId(byte b)
	{
		return new BuildingSpecId() {Id = b};
	}

	public override string ToString()
	{
		return Id.ToString();
	}

	public BuildingSpecId(byte id)
	{
		Id = id;
	}
}

[Serializable]
public class BuildingSpec
{
	public ArtSpec Art;
	public string Description;
	public byte LOS;
	public string Name;
	public BuildingSpecId SpecId;

	public BuildingSpec(byte id)
	{
		SpecId = new BuildingSpecId {Id = id};
	}
}