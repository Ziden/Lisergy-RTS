using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using Game.Systems.Map;
using Game.World;
using LisergyGodotClient.Src.Systems.Movement;
using LisergyGodotClient.Src;
using Game.Engine.Events.Bus;
using LisergyGodotClient.Src.Systems.Visualization;
using GodotClient;
using System.Linq;
using LisergyGodotClient.Src.Systems.Animation;
using LisergyGodotClient.Src.Systems.GameHud;
using System;
using Game.Systems.Movement;

/// <summary>
/// We listen for placement updates to also move the entity on the scene
/// We also place the entity on the logic of the game so we can easily check which entity is in which tile and also take
/// advantage of logical events for the line of sight (exploration)
/// </summary> 
public class EntityPositionListener : IEventListener
{
    private PathVisualizer _pathVisualizer;
    private IClientSdk _client;

    public EntityPositionListener()
    {
        _client = ClientServices.ServerSdk;
        _pathVisualizer = new PathVisualizer();
        ClientServices.Get<IGameObject>().AddChild(new GodotGameObject(_pathVisualizer));
        _client.Server.Entities.OnComponentModified<MapPlacementComponent>(OnComponentModified);
        _client.Server.Entities.OnComponentAdded<MapPlacementComponent>(OnComponentAdded);
        _client.Server.Entities.OnComponentRemoved<MapPlacementComponent>(OnComponentRemoved);
        _client.ClientEvents.On<MovementInterpolationStartEvent>(this, InterpolationStart);
        _client.ClientEvents.On<MovementInterpolationEndEvent>(this, InterpolationEnd);
        _client.ClientEvents.On<EntityMovementRequestStarted>(this, OnMoveRequestStarted);
        _client.ClientEvents.On<ClientPartyActionEvent>(this, OnPartyAction);
    }

    private void OnPartyAction(ClientPartyActionEvent e)
    {
        if (e.Action != EntityAction.MOVE)
        {
            return;
        }
        _client.Server.Actions.MoveEntity(e.TargetEntity, e.TargetTile, Game.Systems.Movement.CourseIntent.Defensive);
    }

    private void OnMoveRequestStarted(EntityMovementRequestStarted e)
    {
        var path = e.Path.Select(p => p.ToGodotVector2()).ToHashSet().ToArray();
        
        _pathVisualizer.DrawPath(path, e.Party.Get<MovespeedComponent>().MoveDelay);
    }

    private void InterpolationStart(MovementInterpolationStartEvent e)
    {
        var view = e.Entity.GetView();
        if (view is IAnimatedSpriteEntity anim)
        {
            anim.UpdateAnimation(e.From.GetDirection(e.To), true);
        }
        _pathVisualizer.StartMovement();
    }

    private void InterpolationEnd(MovementInterpolationEndEvent e)
    {
       // _pathVisualizer.FinishMovement(e.From.Position.ToGodotVector2());
        var view = e.Entity.GetView();
        if (view is IAnimatedSpriteEntity anim)
        {
            if (_pathVisualizer.Remaining == 0 && e.LastStep)
            {
                anim.UpdateAnimation(e.From.GetDirection(e.To), false);
            }
            else
            {
                anim.UpdateAnimation(e.From.GetDirection(e.To), true);
            }
        }
    }

    public void OnComponentRemoved(IEntity entity, MapPlacementComponent oldComponent)
    {
        UpdateEntityPosition(entity, oldComponent, null);
    }

    public void OnComponentModified(IEntity entity, MapPlacementComponent oldComponent, MapPlacementComponent newComponent)
    {
        UpdateEntityPosition(entity, oldComponent, newComponent);
    }

    public void OnComponentAdded(IEntity entity, MapPlacementComponent newComponent)
    {
        UpdateEntityPosition(entity, null, newComponent);
    }

    private void UpdateEntityPosition(IEntity e, MapPlacementComponent oldC, MapPlacementComponent newC)
    {
        var toTile = newC == null ? null : _client.Game.World.GetTile(newC.Position);
        var fromTile = oldC == null ? null : _client.Game.World.GetTile(oldC.Position);
        if (toTile == null) return;
        _client.Game.Log.Debug($"Entity {e} moved from {fromTile} to {toTile}");
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
            view.GameObject.Location = toTile.Position;
        }
    }
}
