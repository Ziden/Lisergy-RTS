using Assets.Code;
using Assets.Code.Assets.Code.Runtime;
using ClientSDK;
using Game;
using Game.ECS;
using Game.Events.Bus;
using GameAssets;
using UnityEngine;

/// <summary>
/// Hook responsible for controlling the entity selector icon that shows which entity the local player have selected
/// </summary>
public class IndicatorSelectedEntityListener : IEventListener
{
    private IGameClient _client;
    private EntitySelectionComponent _selector;

    public IndicatorSelectedEntityListener(IGameClient client)
    {
        _client = client;
        UIEvents.OnSelectEntity += OnEntitySelected;
        _client.UnityServices().Assets.CreateMapObject(MapObjectPrefab.UnitCursor, Vector3.zero, Quaternion.identity, o =>
        {
            o.SetActive(false);
            _selector = o.GetComponent<EntitySelectionComponent>();
            if (ClientState.SelectedEntity != null)
            {
                OnEntitySelected(ClientState.SelectedEntity);
            }
        });
    }

    private void OnEntitySelected(IEntity e)
    {
        if (_selector != null && e is BaseEntity be && be.Tile != null)
        {
            Log.Debug($"Entity Selector Moving to {e}");
            var view = e.GetEntityView() as IGameObject;
            if (view.GameObject == null) return;
            _selector.gameObject.transform.SetParent(view.GameObject.transform, true);
            _selector.gameObject.transform.position = view.GameObject.transform.position;
            if (e.EntityType == EntityType.Building)
            {
                _selector.BuildingRadial.SetActive(true);
                _selector.UnitRadial.SetActive(false);
            }
            else
            {
                _selector.BuildingRadial.SetActive(false);
                _selector.UnitRadial.SetActive(true);
            }
        }
        _selector.gameObject.SetActive(true);
    }
}