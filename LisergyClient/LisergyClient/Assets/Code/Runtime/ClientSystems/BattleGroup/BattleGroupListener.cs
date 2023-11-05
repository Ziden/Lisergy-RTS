using Assets.Code.World;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using GameAssets;

/// <summary>
/// We listen for placement updates to also move the entity on the scene
/// We also place the entity on the logic of the game so we can easily check which entity is in which tile and also take
/// advantage of logical events for the line of sight (exploration)
/// </summary> 
public class BattleGroupListener : BaseComponentListener<BattleGroupComponent>
{
    public BattleGroupListener(IGameClient client) : base(client)
    {
        GameClient.ClientEvents.Register<OwnBattleFinishedEvent>(this, OnLocalPlayerBattleFinished);
    }

    private void OnLocalPlayerBattleFinished(OwnBattleFinishedEvent ev)
    {
        if (ev.Victory) GameClient.UnityServices().Notifications.Display<VictoryNotification>(ev);
        else GameClient.UnityServices().Notifications.Display<DefeatNotification>(ev);
    }

    public override void OnUpdateComponent(IEntity entity, BattleGroupComponent oldComponent, BattleGroupComponent newComponent)
    {
        if (oldComponent.BattleID == GameId.ZERO && newComponent.BattleID != GameId.ZERO)
        {
            _ = GameClient.UnityServices().Vfx.EntityEffects.PlayEffect(entity, VfxPrefab.BattleEffect);
        }
        else if (oldComponent.BattleID != GameId.ZERO && newComponent.BattleID == GameId.ZERO)
        {
            GameClient.UnityServices().Vfx.EntityEffects.StopEffects(entity);
        }
    }

}