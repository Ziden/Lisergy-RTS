using Assets.Code.Assets.Code.Runtime;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Events.Bus;

/// <summary>
/// Shows/hides battle notifications
/// </summary>
public class BattleLIstener : IEventListener
{
    private IGameClient _client;

    public BattleLIstener(IGameClient client)
    {
        _client = client;
        _client.ClientEvents.Register<OwnBattleFinishedEvent>(this, OnBattleFinished);
    }

    private void OnBattleFinished(OwnBattleFinishedEvent ev)
    {
        if (ev.Victory) _client.UnityServices().Notifications.Display<VictoryNotification>(ev);
        else
        {
            _client.UnityServices().Notifications.Display<DefeatNotification>(ev);
        }

    }
}