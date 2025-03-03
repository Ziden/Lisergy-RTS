using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Resources;
using Game.Tile;


/// <summary>
/// Triggered when an entity suposedly harvested one resource from a tile
/// This is a client-sided prediction
/// </summary>
public class HarvestingUpdateEvent : IClientEvent
{
    public HarvestingTaskState InitialState;
    public TileResourceComponent TileResources;
    public int AmountHarvestedTotal;
    public int AmountHarvestedNow;
    public TileModel Tile;
    public IEntity Entity;
    public bool Depleted;
}