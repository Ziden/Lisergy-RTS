using Assets.Code.World;
using ClientSDK;
using Game.ECS;
using Game.Systems.Resources;
using GameAssets;

/// <summary>
/// Listener for harvesting 
/// </summary>
public class HarvestingComponentListener : BaseComponentListener<HarvestingComponent>
{
    public HarvestingComponentListener(IGameClient client) : base(client)
    {
        client.ClientEvents.Register<MovementInterpolationStart>(this, OnMoveStart);
    }

    private void OnMoveStart(MovementInterpolationStart e)
    {
        if (e.Entity.Components.HasReference<HarvestingPredictionComponent>() && !e.Entity.Components.Has<HarvestingComponent>())
        {
            GameClient.Log.Debug($"[HarvestingComponentListener] Moving from {e.From} to {e.To} while harvesting, stopping prediction");
            e.Entity.Components.RemoveReference<HarvestingPredictionComponent>();
        }

    }

    private void OnBeginHarvesting(IEntity entity)
    {
        _ = GameClient.UnityServices().Vfx.EntityEffects.PlayEffect(entity, VfxPrefab.HarvestEffect);
        if (entity.GetEntityView() is PartyView p)
        {
            p.MovementInterpolator.ClearQueue();
        }
        entity.Components.AddReference(new HarvestingPredictionComponent(GameClient, entity));
    }

    private void OnFinishHarvesting(IEntity entity)
    {
        GameClient.Log.Debug("[HarvestingComponentListener] Finishing harvesting");
        GameClient.UnityServices().Vfx.EntityEffects.StopEffects(entity);
        entity.Components.RemoveReference<HarvestingPredictionComponent>();
    }

    public override void OnUpdateComponent(IEntity entity, HarvestingComponent oldComponent, HarvestingComponent newComponent)
    {
        if (oldComponent.StartedAt == 0 && newComponent.StartedAt > 0) OnBeginHarvesting(entity);
        else if (oldComponent.StartedAt > 0 && newComponent.StartedAt == 0) OnFinishHarvesting(entity);
    }
}