using ClientSDK.Data;
using Game.ECS;
using Game.Engine.ECS;

/// <summary>
/// Event for when the local player selects one of his local entities
/// </summary>
public class OwnEntitySelected : IClientEvent
{
    public IEntity Entity;
}