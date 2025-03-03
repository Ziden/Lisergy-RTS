
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Tile;

/// <summary>
/// Triggered when movement interpolation ended
/// </summary>
public class MovementInterpolationEnd : IClientEvent
{
    public TileModel From;
    public TileModel To;
    public IEntity Entity;
    public bool LastStep;
}