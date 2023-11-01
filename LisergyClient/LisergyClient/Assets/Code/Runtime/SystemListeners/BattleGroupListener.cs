using Assets.Code.World;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Party;
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
        Client.ClientEvents.Register<OwnBattleFinishedEvent>(this, OnLocalPlayerBattleFinished);
    }

    private void OnLocalPlayerBattleFinished(OwnBattleFinishedEvent ev)
    {
        if (ev.Victory) Client.UnityServices().Notifications.Display<VictoryNotification>(ev);
        else Client.UnityServices().Notifications.Display<DefeatNotification>(ev);
    }

    public override void OnUpdateComponent(IEntity entity, BattleGroupComponent oldComponent, BattleGroupComponent newComponent)
    {
        if (oldComponent.BattleID == GameId.ZERO && newComponent.BattleID != GameId.ZERO)
        {
            Client.UnityServices().Vfx.EntityEffects.PlayEffect(entity, MapFX.BattleEffect);
        }
        else if (oldComponent.BattleID != GameId.ZERO && newComponent.BattleID == GameId.ZERO)
        {
            Client.UnityServices().Vfx.EntityEffects.StopEffects(entity);
        }
    }

}