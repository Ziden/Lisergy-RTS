using Assets.Code.World;
using ClientSDK;
using Game.Engine.ECLS;
using Game.Systems.Resources;
using GameAssets;

/// <summary>
/// Listener for harvesting 
/// </summary>
public class HarvestingComponentListener : BaseComponentListener<HarvestingComponent>
{
    public HarvestingComponentListener(IGameClient client) : base(client)
    {
        client.ClientEvents.On<MovementInterpolationStart>(this, OnMoveStart);
    }

    private void OnMoveStart(MovementInterpolationStart e)
    {
        if (e.Entity.Components.Has<HarvestingPredictionComponent>() && !e.Entity.Components.Has<HarvestingComponent>())
        {
            GameClient.Log.Debug($"[HarvestingComponentListener] Moving from {e.From} to {e.To} while harvesting, stopping prediction");
            e.Entity.Components.Remove<HarvestingPredictionComponent>();
        }
    }

    private void OnBeginHarvesting(IEntity entity)
    {
        _ = GameClient.UnityServices().Vfx.EntityEffects.PlayEffect(entity, VfxPrefab.HarvestEffect);
        if (entity.GetView() is PartyView p)
        {
            p.MovementInterpolator.ClearQueue();
        }
        var c = new HarvestingPredictionComponent();
        c.StartTracking(GameClient, entity);
        entity.Components.Add(c);
    }

    private void OnFinishHarvesting(IEntity entity)
    {
        GameClient.Log.Debug("[HarvestingComponentListener] Finishing harvesting");
        GameClient.UnityServices().Vfx.EntityEffects.StopEffects(entity);
        entity.Components.Remove<HarvestingPredictionComponent>();
    }

    public override void OnUpdateComponent(IEntity entity, HarvestingComponent oldComponent, HarvestingComponent newComponent)
    {
        if (oldComponent.StartedAt == 0 && newComponent.StartedAt > 0) OnBeginHarvesting(entity);
        else if (oldComponent.StartedAt > 0 && newComponent.StartedAt == 0) OnFinishHarvesting(entity);
    }

    public override void OnComponentRemoved(IEntity entity, HarvestingComponent oldComponent)
    {
        OnFinishHarvesting(entity);
    }
}