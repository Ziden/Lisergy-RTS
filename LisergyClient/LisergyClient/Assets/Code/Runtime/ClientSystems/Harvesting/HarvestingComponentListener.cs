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
    {}

    private void OnBeginHarvesting(IEntity entity)
    {
        _ = GameClient.UnityServices().Vfx.EntityEffects.PlayEffect(entity, VfxPrefab.HarvestEffect);
        entity.Components.AddReference(new HarvestingPredictionComponent(GameClient, entity));
    }

    private void OnFinishHarvesting(IEntity entity)
    {
        GameClient.UnityServices().Vfx.EntityEffects.StopEffects(entity);
        entity.Components.RemoveReference<HarvestingPredictionComponent>();
    }

    public override void OnUpdateComponent(IEntity entity, HarvestingComponent oldComponent, HarvestingComponent newComponent)
    {
        if (oldComponent.StartedAt == 0 && newComponent.StartedAt > 0) OnBeginHarvesting(entity);
        else if (oldComponent.StartedAt > 0 && newComponent.StartedAt == 0) OnFinishHarvesting(entity);
    }
}