
using ClientSDK.Data;
using Game.ECS;
using Game.Tile;

/// <summary>
/// Triggered when movement interpolation started
/// </summary>
public class MovementInterpolationStart : IClientEvent
{
    public TileEntity From;
    public TileEntity To;
    public IEntity Entity;
}