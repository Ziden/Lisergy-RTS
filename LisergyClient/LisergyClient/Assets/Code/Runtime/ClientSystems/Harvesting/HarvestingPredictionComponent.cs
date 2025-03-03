
using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.Resources;
using Game.Tile;
using GameData;
using System;

/// <summary>
/// Predicts how much and when would harvest new resources and send client events based on its predictions
/// </summary>
[Serializable]
public class HarvestingPredictionComponent : IComponent, IDisposable
{
    private IEntity _entity;
    private TileModel _tile;
    private TileResourceComponent _initialComponent;
    private ResourceSpec _resourceSpec;
    private ResourceHarvestPointSpec _harvestSpec;
    private IGameClient _client;
    private bool _tracking = false;
    private ushort _harvestedTotal;

    public void StartTracking(IGameClient client, IEntity harvester)
    {
        _client = client;
        _entity = harvester;
        var tilePosition = _entity.Get<HarvestingComponent>().Tile;
        _tile = client.Game.World.GetTile(tilePosition.X, tilePosition.Y);
        _initialComponent = _tile.Get<TileResourceComponent>();
        _resourceSpec = _entity.Game.Specs.Resources[_initialComponent.Resource.ResourceId];
        _harvestSpec = _tile.HarvestPointSpec;
        _tracking = true;
        _ = TrackerTask();
    }

    public void Dispose()
    {
        if (_tracking)
        {
            _client.Game.Log.Debug($"[Harvest Prediction] Disposing: Stopping prediction");
            _tracking = false;
            NotifyFinished();
        }

    }

    private void NotifyFinished()
    {
        _client.UnityServices().Notifications.Display<FinishedHarvestingNotification>(new FinishedHarvestingParam()
        {
            Resource = new ResourceStackData(_initialComponent.Resource.ResourceId, _harvestedTotal),
            Entity = _entity.GetView(),
        });
    }

    private async UniTaskVoid TrackerTask()
    {  
        var harvestState = _entity.Logic.Harvesting.CalculateCurrentState();
        var nextHarvest = harvestState.TimeSnapshot.TimeBlock.StartTime + _harvestSpec.HarvestTimePerUnit;
        _harvestedTotal = 0;
        if (nextHarvest < _client.Game.GameTime)
        {
            _harvestedTotal = (ushort)Math.Floor((_client.Game.GameTime - harvestState.TimeSnapshot.TimeBlock.StartTime) / _harvestSpec.HarvestTimePerUnit);
            if (_harvestedTotal > _initialComponent.Resource.Amount) _harvestedTotal = _initialComponent.Resource.Amount;

            nextHarvest = harvestState.TimeSnapshot.TimeBlock.StartTime + (_harvestSpec.HarvestTimePerUnit * (_harvestedTotal + 1));
            _client.Game.Log.Debug($"[Harvest Prediction] Was harvesting already for some time - Harvested Total Start {_harvestedTotal}");
        }
        var depleted = _harvestedTotal >= _initialComponent.Resource.Amount;
        SendEvent(0, _harvestedTotal, depleted, harvestState);
        while (_tracking)
        {
            _client.Game.Log.Debug($"[Harvest Prediction] Waiting prediction next tick: {nextHarvest} now {_client.Game.GameTime}");
            await UniTask.Delay(nextHarvest - _client.Game.GameTime);
            if (!_tracking) return;
            if (depleted) return;

            nextHarvest += _harvestSpec.HarvestTimePerUnit;
            _harvestedTotal++;
            depleted = _harvestedTotal >= _initialComponent.Resource.Amount;
            SendEvent(1, _harvestedTotal, depleted, harvestState);
            _client.Game.Log.Debug($"[Harvest Prediction] Harvested 1 -> {_harvestedTotal}/{_initialComponent.Resource.Amount}");
        }
    }
    
    private void SendEvent(int amountHarvested, int harvestedTotal, bool depleted, in HarvestingTaskState initialState)
    {
        var ev = EventPool<HarvestingUpdateEvent>.Get();
        ev.TileResources = _initialComponent;
        ev.Tile = _tile;
        ev.AmountHarvestedNow = amountHarvested;
        ev.AmountHarvestedTotal = harvestedTotal;
        ev.InitialState = initialState;
        ev.Entity = _entity;
        ev.Depleted = depleted;
        _client.ClientEvents.Call(ev);
        EventPool<HarvestingUpdateEvent>.Return(ev);
    }
}