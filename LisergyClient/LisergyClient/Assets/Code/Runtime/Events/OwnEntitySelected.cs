using ClientSDK.Data;
using Game.Engine.ECLS;


/// <summary>
/// Event for when the local player selects one of his local entities
/// </summary>
public class OwnEntitySelected : IClientEvent
{
    public IEntity Entity;
}