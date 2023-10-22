using Assets.Code.Assets.Code.Runtime.UIScreens;
using Game.Battle.Data;
using GameAssets;
using UnityEngine.UIElements;

public class VictoryNotification : Notification
{
    public override UIScreen ScreenAsset => UIScreen.VictoryNotification;


    public override void OnOpen()
    {
        base.OnOpen();
        var param = GetParameter<BattleHeaderData>();
        var mine = Root.Q<Label>("Mine").Required();
        var enemy = Root.Q<Label>("Enemy").Required();
        if(param.Attacker.OwnerID.IsMine())
        {
            mine.text = GameClient.Game.Specs.Units[param.Attacker.Units.Leader.SpecId].Name;
            enemy.text = GameClient.Game.Specs.Units[param.Defender.Units.Leader.SpecId].Name;
        }
        else if (param.Defender.OwnerID.IsMine())
        {
            enemy.text = GameClient.Game.Specs.Units[param.Attacker.Units.Leader.SpecId].Name;
            mine.text = GameClient.Game.Specs.Units[param.Defender.Units.Leader.SpecId].Name;
        }
    }
}