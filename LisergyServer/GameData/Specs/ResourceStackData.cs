using System;
using System.Runtime.InteropServices;
using GameData;

namespace Game.Systems.Resources;

/// <summary>
///     Represents a resource stack, that's a resource type and an amount
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct ResourceStackData
{
	private ushort _amount;

	public ushort Amount
	{
		get => _amount;
		set => _amount = value;
	}

	public ResourceSpecId ResourceId { get; private set; }

	public ResourceStackData(in ResourceSpecId id, in ushort amount)
	{
		ResourceId = id;
		_amount = amount;
	}

	public bool CanAdd(in ResourceStackData data)
	{
		return Empty || ResourceId == data.ResourceId;
	}

	public void Add(in ResourceStackData stack)
	{
		if (ResourceId != stack.ResourceId)
		{
			if (_amount > 0) throw new Exception("Cannot add stack to resource stack of different type when not empty");
			ResourceId = stack.ResourceId;
		}

		_amount += stack._amount;
	}

	public bool Empty => _amount == 0;

	public override string ToString()
	{
		return $"<Resource Id={ResourceId} Amt={_amount}>";
	}
}