using Assets.Code;
using Assets.Code.UI;
using ClientSDK;
using Game;
using Game.ECS;
using Game.Systems.Map;
using UnityEngine;

/// <summary>
/// We listen for placement updates to also move the entity on the scene
/// We also place the entity on the logic of the game so we can easily check which entity is in which tile and also take
/// advantage of logical events for the line of sight (exploration)
/// </summary> 
public class MapPlacementComponentListener : BaseComponentListener<MapPlacementComponent>  
{
    private MovePathListener _movementPath;

    public MapPlacementComponentListener(IGameClient client) : base(client) {

        _movementPath = new MovePathListener(client);
    }

    public override void OnUpdateComponent(IEntity entity, MapPlacementComponent oldComponent, MapPlacementComponent newComponent)
    {
        var newTile = Client.Game.World.Map.GetTile(newComponent.Position.X, newComponent.Position.Y);
        Log.Debug($"Setting position of entity {entity}");
        entity.EntityLogic.Map.SetPosition(newTile);
        _movementPath.OnFinishedMove(entity, newTile);
        if (Client.Modules.Views.GetEntityView(entity) is IGameObject o && o.GameObject != null)
        {
            o.GameObject.transform.position = new Vector3(newComponent.Position.X, o.GameObject.transform.position.y, newComponent.Position.Y);
        }
    }
}