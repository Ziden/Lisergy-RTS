using System;
using System.Runtime.InteropServices;
using GameData.Specs;

namespace GameData;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct TileSpecId
{
	public byte Id;

	public static implicit operator byte(TileSpecId d)
	{
		return d.Id;
	}

	public static implicit operator TileSpecId(byte b)
	{
		return new TileSpecId {Id = b};
	}

	public TileSpecId(byte id)
	{
		Id = id;
	}

	public override string ToString()
	{
		return Id.ToString();
	}
}

[Serializable]
public class TileSpec
{
	/// <summary>
	///     Change to tile id when resource is depleted
	/// </summary>
	public TileSpecId ChangeToTileIdWhenDepleted;

	public ArtSpec Icon;
	public TileSpecId ID;
	public ArtSpec Model;

	// 1=passable, 0=impassable, 0.5% slower
	public float MovementFactor;
	public string Name;

	/// <summary>
	///     Any resources that are always present on this tile id
	/// </summary>
	public HarvestPointSpecId? ResourceSpotSpecId;

	public TileSpec(in byte i)
	{
		ID = new TileSpecId {Id = i};
		Model = default;
		MovementFactor = 1.0f;
	}
}