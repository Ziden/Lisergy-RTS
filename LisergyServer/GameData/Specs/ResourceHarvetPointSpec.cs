using System;
using System.Runtime.InteropServices;

namespace GameData;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct HarvestPointSpecId
{
	public byte Id;

	public static implicit operator byte(HarvestPointSpecId d)
	{
		return d.Id;
	}

	public static implicit operator HarvestPointSpecId(byte b)
	{
		return new HarvestPointSpecId() {Id = b};
	}

	public override string ToString()
	{
		return Id.ToString();
	}
}

/// <summary>
///     Represents a harvestable resource on a given tile
/// </summary>
[Serializable]
public class ResourceHarvestPointSpec
{
    /// <summary>
    ///     Harvest time per 1 unit of the resource
    /// </summary>
    public TimeSpan HarvestTimePerUnit;

    /// <summary>
    ///     Max amount of resource to spawn
    /// </summary>
    public ushort ResourceAmount;

    /// <summary>
    ///     What resource to spawn
    /// </summary>
    public ResourceSpecId ResourceId;

    /// <summary>
    ///     Time to respawn every unit of the given resource after its stopped being harvested
    /// </summary>
    public TimeSpan RespawnTime;

    /// <summary>
    ///     Spec id
    /// </summary>
    public HarvestPointSpecId SpecId;

	public ResourceHarvestPointSpec(in byte i)
	{
		SpecId = new HarvestPointSpecId {Id = i};
	}
}