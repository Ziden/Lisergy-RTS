using Game.Systems.Resources;
using System;
using System.Threading.Tasks;
using ClientSDK;
using Game.Engine.DataTypes;

/// <summary>
/// Predicts how much and when would harvest new resources and send client events based on its predictions
/// </summary>
[Serializable]
public struct TimeBlockTask : IDisposable
{
	private TimeBlock _timeBlock;
	private Action? _onComplete;
	private TimeSpan _delay;
	
	public TimeBlockTask(IClientSdk sdk, TimeBlock timeBlock, Action callback)
	{
		_timeBlock = timeBlock;
		_onComplete = callback;
		_delay = _timeBlock.TimeTillCompletion(sdk.Game);
		_ = CreateTask();
	}

	private async Task CreateTask()
	{
		await Task.Delay(_delay);
		_onComplete?.Invoke();
	}
	
	public void Dispose()
	{
		_onComplete = null;
	}
}