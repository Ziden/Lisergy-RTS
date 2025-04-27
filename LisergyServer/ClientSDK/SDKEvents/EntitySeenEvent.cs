using ClientSDK.Data;
using Game.Engine.ECLS;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ClientSDK.SDKEvents;

/// <summary>
///     Fired when an entity is seen and not known by client
/// </summary>
public class EntitySeenEvent : IClientEvent
{
	public IEntity Entity;

	/// <summary>
	///     Fired when an entity is seen and not known by client
	/// </summary>
	public EntitySeenEvent(IEntity e)
	{
		Entity = e;
	}
}