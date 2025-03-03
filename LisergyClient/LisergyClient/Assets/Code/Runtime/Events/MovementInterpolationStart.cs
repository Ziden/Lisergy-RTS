
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Tile;

/// <summary>
/// Triggered when movement interpolation started
/// </summary>
public class MovementInterpolationStart : IClientEvent
{
    public TileModel From;
    public TileModel To;
    public IEntity Entity;
}