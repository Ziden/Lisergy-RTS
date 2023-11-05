using Assets.Code.UI;
using Assets.Code.World;
using ClientSDK;
using Game.Events.Bus;
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
        client.Game.Events.Register<EntityMoveInEvent>(this, OnEntityMoveIn);
    }

    private void OnEntityMoveIn(EntityMoveInEvent ev)
    {
        if (ev.ToTile == null) return;
        _client.Game.Log.Debug($"Entity {ev.Entity} moved from {ev.FromTile} to {ev.ToTile}");
        _movementPath.OnFinishedMove(ev.Entity, ev.ToTile);
        var view = ev.Entity.GetEntityView() as PartyView;
        if (view == null) return;
        if (ev.FromTile != null && ev.ToTile.Distance(ev.FromTile) <= 1)
        {
            view.MovementInterpolator.InterpolateMovement(ev.FromTile, ev.ToTile);
        }
        else
        {
            view.GameObject.transform.position = ev.ToTile.UnityPosition();
        }
    }
}