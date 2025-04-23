using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Tile;

/// <summary>
/// Triggered when movement interpolation ended
/// </summary>
public class MovementInterpolationEndEvent : IClientEvent
{
	public required IEntity Entity;
	public required TileModel From;
	public required bool LastStep;
	public required TileModel To;
}