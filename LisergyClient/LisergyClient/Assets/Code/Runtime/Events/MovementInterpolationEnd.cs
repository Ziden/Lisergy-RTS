
using ClientSDK.Data;
using Game.ECS;
using Game.Tile;

/// <summary>
/// Triggered when movement interpolation ended
/// </summary>
public class MovementInterpolationEnd : IClientEvent
{
    public TileEntity From;
    public TileEntity To;
    public IEntity Entity;
    public bool LastStep;
}