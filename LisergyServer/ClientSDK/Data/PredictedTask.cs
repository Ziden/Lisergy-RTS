using Game.Systems.Resources;
using System;
using System.Threading.Tasks;
using ClientSDK;
using Game.Engine.DataTypes;

/// <summary>
/// Predicts how much and when would harvest new resources and send client events based on its predictions
/// </summary>
[Serializable]
public class PredictionTask : IDisposable
{
    public event Action<TimeBlockSnapshot>? OnTick;
    public event Action<TimeBlockSnapshot>? OnFinish;
    
    private TimeBlock _timeBlock;
    private bool _tracking;
    private readonly IClientSdk _client;
    private TimeSpan _tickTime;
    
    public PredictionTask(IClientSdk client, TimeBlock time, TimeSpan tickTime = default)
    {
        if(tickTime == default) tickTime = TimeSpan.FromSeconds(0.1);
        _tickTime = tickTime;
        _client = client;
        _timeBlock = time;
       _ = TrackerTask();
    }

    public void Dispose()
    {
        if (_tracking)
        {
            _client.Game.Log.Debug($"Disposing: Stopping prediction");
            _tracking = false;
            OnFinish?.Invoke(_timeBlock.GetCurrentSnapshot(_client.Game.GameTime));
        }
    }

    private async Task TrackerTask()
    {
        var endTime = _timeBlock.EndTime;
        var now = _client.Game.GameTime;
        if (now > endTime)
        {
            OnFinish?.Invoke(_timeBlock.GetCurrentSnapshot(_client.Game.GameTime));
            return;
        }
        while (_tracking && _client.Game.GameTime < endTime)
        {
            await Task.Delay(_tickTime);
            OnTick?.Invoke(_timeBlock.GetCurrentSnapshot(_client.Game.GameTime));
        }
    }
    
    /*
    private async Task TrackerTask()
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
    */

    private void SendEvent(int amountHarvested, int harvestedTotal, bool depleted, in HarvestingTaskState initialState)
    {
        /*
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
        */
    }
}