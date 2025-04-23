using System;
using System.Runtime.InteropServices;
using GameData.Specs;

namespace GameData;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct ResourceSpecId
{
	public byte Id;

	public static implicit operator byte(ResourceSpecId d)
	{
		return d.Id;
	}

	public static implicit operator ResourceSpecId(byte b)
	{
		return new ResourceSpecId() {Id = b};
	}

	public override string ToString()
	{
		return Id.ToString();
	}
}

[Serializable]
public class ResourceSpec
{
	public ArtSpec Art;
	public string Name;
	public bool ShowInUi = false;
	public ResourceSpecId SpecId;
	public byte WeightPerUnit;

	public ResourceSpec(byte i)
	{
		SpecId = new ResourceSpecId {Id = i};
		Art = default;
	}
}