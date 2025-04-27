using System;
using System.Runtime.InteropServices;
using Game.Specs;

namespace GameData.Specs;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct UnitSpecId
{
	public byte Id;

	public static implicit operator byte(UnitSpecId d)
	{
		return d.Id;
	}

	public static implicit operator UnitSpecId(byte b)
	{
		return new UnitSpecId {Id = b};
	}

	public override string ToString()
	{
		return Id.ToString();
	}

	public UnitSpecId(byte id)
	{
		Id = id;
	}
}

[Serializable]
public class UnitSpec
{
	public ArtSpec Art;
	public ArtSpec IconArt;
	public byte LOS;
	public string Name;
	public UnitSpecId SpecId;
	public UnitStats Stats;
}