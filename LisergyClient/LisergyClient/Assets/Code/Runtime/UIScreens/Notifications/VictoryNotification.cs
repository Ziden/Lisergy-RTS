using Assets.Code.Assets.Code.Runtime.UIScreens;
using ClientSDK.SDKEvents;
using Game.Battle.Data;
using GameAssets;
using UnityEngine.UIElements;

public class VictoryNotification : Notification
{
    public override UIScreen UiAsset => UIScreen.VictoryNotification;

    public override void OnOpen()
    {
        base.OnOpen();
        var param = GetParameter<OwnBattleFinishedEvent>();
        var mine = Root.Q<Label>("Mine").Required();
        var enemy = Root.Q<Label>("Enemy").Required();
        mine.text = GameClient.Game.Specs.Units[param.MyTeam.Units.Leader.SpecId].Name;
        enemy.text = GameClient.Game.Specs.Units[param.EnemyTeam.Units.Leader.SpecId].Name;
    }
}