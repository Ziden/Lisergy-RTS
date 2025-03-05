using Assets.Code.Assets.Code.Runtime.UIScreens;
using ClientSDK.SDKEvents;
using GameAssets;
using UnityEngine.UIElements;

public class DefeatNotification : Notification
{
    public override UIScreen UiAsset => UIScreen.DefeatNotification;

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