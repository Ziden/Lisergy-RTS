using System;
using System.Collections.Generic;
using ClientSDK.SDKEvents;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Engine.Network;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ClientSDK.Autoplay;

public class BrainTask
{
	internal bool Sent = false;
	public BasePacket Command;
	public Func<bool> IsComplete;
}

public class AutoplayController : IEventListener
{
	private readonly IClientSdk _sdk;
	private Dictionary<GameId, AutoplayBrain> _brains = new();

	public AutoplayController(IClientSdk sdk)
	{
		_sdk = sdk;
	}
	
	public void Tick()
	{
		foreach(var v in _brains.Values) v.Tick();
	}

	public AutoplayBrain GetBrain(IEntity e)
	{
		if (e.OwnerID != _sdk.Modules.Player.PlayerId) return null!;
		if (!_brains.TryGetValue(e.EntityId, out var brain))
		{
			_brains[e.EntityId] = new AutoplayBrain(_sdk);
			_sdk.Log.Info("Brain added for entity "+e);
		}
		return _brains[e.EntityId];
	}
}

public class AutoplayBrain
{
	private Queue<BrainTask> _commandQueue = new();
	private IClientSdk _sdk;

	public AutoplayBrain(IClientSdk sdk)
	{
		_sdk = sdk;
	}

	public void Tick()
	{
		if (_commandQueue.TryPeek(out var o))
		{
			if (!o.Sent)
			{
				o.Sent = true;
				_sdk.Network.SendToServer(o.Command);
			}
			else if (o.IsComplete())
			{
				_commandQueue.Dequeue();
			}
		}
	}
	
	public void Add(BrainTask task)
	{
		_commandQueue.Enqueue(task);
	}
}