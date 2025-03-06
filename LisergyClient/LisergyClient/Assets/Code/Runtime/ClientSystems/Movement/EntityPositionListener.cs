using Assets.Code.Assets.Code.Runtime.Movement;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using Game.Systems.Map;
using Game.World;

/// <summary>
/// We listen for placement updates to also move the entity on the scene
/// We also place the entity on the logic of the game so we can easily check which entity is in which tile and also take
/// advantage of logical events for the line of sight (exploration)
/// </summary> 
public class EntityPositionListener : BaseComponentListener<MapPlacementComponent>
{
    private new MovePathLogic _movementPath;
    private IGameClient _client;

    public EntityPositionListener(IGameClient client) : base(client)
    {
        _client = client;
        _movementPath = new MovePathLogic(client);
        _client.ClientEvents.On<MovementInterpolationEndEvent>(this, e => _movementPath.OnFinishedMove(e.Entity, e.To.Entity));
        _client.ClientEvents.On<EntityMovementRequestStarted>(this, e => _movementPath.StartMovement(e).Forget());
    }

    public override void OnComponentRemoved(IEntity entity, MapPlacementComponent oldComponent)
    {
        UpdateEntityPosition(entity, oldComponent, null);
    }

    public override void OnComponentModified(IEntity entity, MapPlacementComponent oldComponent, MapPlacementComponent newComponent)
    {
        UpdateEntityPosition(entity, oldComponent, newComponent);
    }

    public override void OnComponentAdded(IEntity entity, MapPlacementComponent newComponent)
    {
        UpdateEntityPosition(entity, null, newComponent);
    }

    private void UpdateEntityPosition(IEntity e, MapPlacementComponent oldC, MapPlacementComponent newC)
    {
        var toTile = newC == null ? null : _client.Game.World.GetTile(newC.Position);
        var fromTile = oldC == null ? null : _client.Game.World.GetTile(oldC.Position);
        if (toTile == null) return;
        _client.Game.Log.Debug($"Entity {e} moved from {fromTile} to {toTile}");
        _movementPath.OnFinishedMove(e, toTile.Entity);
        var view = e.GetView();
        if (view == null)
        {
            _client.Game.Log.Error($"Entity {e} have view");
            return;
        }
        if (view is IEntityMovementInterpolated i && fromTile != null && toTile.Distance(fromTile) <= 1)
        {
            i.MovementInterpolator.InterpolateMovement(fromTile, toTile);
        }
        else if (view.Entity.Components.Has<MapPlacementComponent>() && view.State == ClientSDK.Data.EntityViewState.RENDERED)
        {
            view.GameObject.transform.position = toTile.UnityPosition();
        }

    }
}
