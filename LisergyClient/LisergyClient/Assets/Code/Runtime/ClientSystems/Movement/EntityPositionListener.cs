using Assets.Code.Assets.Code.Runtime.Movement;
using Assets.Code.UI;
using Assets.Code.World;
using ClientSDK;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Systems.Map;
using Game.World;

/// <summary>
/// We listen for placement updates to also move the entity on the scene
/// We also place the entity on the logic of the game so we can easily check which entity is in which tile and also take
/// advantage of logical events for the line of sight (exploration)
/// </summary> 
public class EntityPositionListener : IEventListener
{
    private MovePathListener _movementPath;
    private IGameClient _client;

    public EntityPositionListener(IGameClient client)
    {
        _client = client;
        _movementPath = new MovePathListener(client);
        client.Modules.Entities.OnComponentUpdate<MapPlacementComponent>(OnEntityMoveIn);
    }

    private void OnEntityMoveIn(IEntity e, MapPlacementComponent oldC, MapPlacementComponent newC)
    {
        var toTile = newC == null ? null : _client.Game.World.GetTile(newC.Position);
        var fromTile = oldC == null ? null : _client.Game.World.GetTile(oldC.Position);
        if (toTile == null) return;
        _client.Game.Log.Debug($"Entity {e} moved from {fromTile} to {toTile}");
        _movementPath.OnFinishedMove(e, toTile);
        var view = e.GetView();
        if (view == null) return;
        if (view is IEntityMovementInterpolated i && fromTile != null && toTile.Distance(fromTile) <= 1)
        {
            i.MovementInterpolator.InterpolateMovement(fromTile, toTile);
        }
        else if(view.Entity.Components.Has<MapPlacementComponent>() && view.State == ClientSDK.Data.EntityViewState.RENDERED)
        {
            view.GameObject.transform.position = toTile.UnityPosition();
        }
    }
}