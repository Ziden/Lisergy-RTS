
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
    private TileResourceComponent _initialComponent;
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
        _initialComponent = _tile.Get<TileResourceComponent>();
        _resourceSpec = _entity.Game.Specs.Resources[_initialComponent.Resource.ResourceId];
        _harvestSpec = _tile.HarvestPointSpec;
        _tracking = true;
        _ = TrackerTask();
    }

    public void Dispose()
    {
        _client.Game.Log.Debug($"[Harvest Prediction] Disposing: Stopping prediction");
        _tracking = false;
        NotifyFinished();
    }

    private void NotifyFinished()
    {
        _client.UnityServices().Notifications.Display<FinishedHarvestingNotification>(new FinishedHarvestingParam()
        {
            Resource = _initialComponent.Resource,
            Entity = _entity.GetEntityView() as IUnityEntityView,
        });
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
            if (harvestedTotal > _initialComponent.Resource.Amount) harvestedTotal = _initialComponent.Resource.Amount;

            nextHarvest = startedDate + (_harvestSpec.HarvestTimePerUnit * (harvestedTotal + 1));
            _client.Game.Log.Debug($"[Harvest Prediction] Was harvesting already for some time - Harvested Total Start {harvestedTotal}");
        }
        var depleted = harvestedTotal >= _initialComponent.Resource.Amount;
        SendEvent(0, harvestedTotal, depleted);
        while (_tracking)
        {
            _client.Game.Log.Debug($"[Harvest Prediction] Waiting prediction next tick: {nextHarvest} now {DateTime.UtcNow}");
            await UniTask.Delay(nextHarvest - DateTime.UtcNow);
            if (!_entity.Components.Has<HarvestingComponent>())
            {
                _client.Game.Log.Debug($"Cancelling harvesting as have no component");
                Dispose();
                return;
            }
            nextHarvest += _harvestSpec.HarvestTimePerUnit;
            harvestedTotal++;
            depleted = harvestedTotal >= _initialComponent.Resource.Amount;
            SendEvent(1, harvestedTotal, depleted);
            _client.Game.Log.Debug($"[Harvest Prediction] Harvested 1 -> {harvestedTotal}/{_initialComponent.Resource.Amount}");
            if (depleted)
            {
                _client.Game.Log.Debug($"[Harvest Prediction] Client predicted depletion on {_tile}");
                Dispose();
            }
        }
    }
    
    private void SendEvent(int amountHarvested, int harvestedTotal, bool depleted)
    {
        var ev = EventPool<HarvestingUpdateEvent>.Get();
        ev.TileResources = _initialComponent;
        ev.Tile = _tile;
        ev.AmountHarvestedNow = amountHarvested;
        ev.AmountHarvestedTotal = harvestedTotal;
        ev.Entity = _entity;
        ev.Depleted = depleted;
        _client.ClientEvents.Call(ev);
        EventPool<HarvestingUpdateEvent>.Return(ev);
    }
}