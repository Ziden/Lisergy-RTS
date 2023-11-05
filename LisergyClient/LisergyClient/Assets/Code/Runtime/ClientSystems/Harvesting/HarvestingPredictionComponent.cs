
using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.ECS;
using Game.Events;
using Game.Systems.Resources;
using Game.Tile;
using GameData;
using System;

/// <summary>
/// Predicts how much and when would harvest new resources and send client events based on its predictions
/// </summary>
public class HarvestingPredictionComponent : IReferenceComponent, IDisposable
{
    private IEntity _entity;
    private TileEntity _tile;
    private TileResourceComponent _resources;
    private ResourceSpec _resourceSpec;
    private ResourceHarvestPointSpec _harvestSpec;
    private IGameClient _client;
    private bool _tracking = false;

    public HarvestingPredictionComponent(IGameClient client, IEntity harvester)
    {
        _client = client;
        _entity = harvester;
        var tilePosition = _entity.Get<HarvestingComponent>().Tile;
        _tile = client.Game.World.Map.GetTile(tilePosition.X, tilePosition.Y);
        _resources = _tile.Get<TileResourceComponent>();
        _resourceSpec = _entity.Game.Specs.Resources[_resources.ResourceId];
        _harvestSpec = _tile.HarvestPointSpec;
        _tracking = true;
        _ = TrackerTask();
    }

    public void Dispose()
    {
        _tracking = false;
    }

    private async UniTaskVoid TrackerTask()
    {
        var harvesting = _entity.Get<HarvestingComponent>();
        var startedDate = DateTime.FromBinary(harvesting.StartedAt);
        var nextHarvest = startedDate + _harvestSpec.HarvestTimePerUnit;
        var harvestedTotal = 0;
        if (nextHarvest < DateTime.UtcNow)
        {
            harvestedTotal = (int)Math.Floor((DateTime.UtcNow - startedDate) / _harvestSpec.HarvestTimePerUnit);
            nextHarvest = startedDate + (_harvestSpec.HarvestTimePerUnit * (harvestedTotal + 1));
            _client.Game.Log.Debug($"Harvested Total Start {harvestedTotal}");
        }
        var depleted = harvestedTotal >= _resources.AmountResourcesLeft;
        while (_tracking)
        {
            await UniTask.Delay(nextHarvest - DateTime.UtcNow);
            nextHarvest += _harvestSpec.HarvestTimePerUnit;
            harvestedTotal++;
            depleted = harvestedTotal >= _resources.AmountResourcesLeft;

            var ev = EventPool<HarvestedResourceEvent>.Get();
            ev.TileResources = _resources;
            ev.Tile = _tile;
            ev.AmountHarvestedNow = 1;
            ev.AmountHarvestedTotal = harvestedTotal;
            ev.Entity = _entity;
            ev.Depleted = depleted;
            _client.ClientEvents.Call(ev);
            EventPool<HarvestedResourceEvent>.Return(ev);
            _client.Game.Log.Debug($"Harvested 1 -> {harvestedTotal}/{_resources.AmountResourcesLeft}");
            if (depleted) Dispose();
        }
    }
}