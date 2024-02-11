using ClientSDK.Data;
using Game.ECS;
using Game.Systems.Resources;
using Game.Tile;


/// <summary>
/// Triggered when an entity suposedly harvested one resource from a tile
/// This is a client-sided prediction
/// </summary>
public class HarvestingUpdateEvent : IClientEvent
{
    public TileResourceComponent TileResources;
    public int AmountHarvestedTotal;
    public int AmountHarvestedNow;
    public TileEntity Tile;
    public IEntity Entity;
    public bool Depleted;
}