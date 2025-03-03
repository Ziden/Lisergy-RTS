using Assets.Code;
using Assets.Code.Assets.Code.Runtime;
using ClientSDK;
using Game.Entities;
using Game.Engine.Events.Bus;
using GameAssets;
using UnityEngine;

/// <summary>
/// Hook responsible for controlling the entity selector icon that shows which entity the local player have selected
/// </summary>
public class IndicatorSelectedPartyListener : IEventListener
{
    private IGameClient _client;
    private EntitySelectionMonoBehaviour _selector;

    public IndicatorSelectedPartyListener(IGameClient client)
    {
        _client = client;
        ClientViewState.OnSelectEntity += OnEntitySelected;
        _client.UnityServices().Assets.CreateMapObject(MapObjectPrefab.UnitCursor, Vector3.zero, Quaternion.identity, o =>
        {
            o.SetActive(false);
            _selector = o.GetComponent<EntitySelectionMonoBehaviour>();
            if (ClientViewState.SelectedEntityView != null)
            {
                OnEntitySelected(ClientViewState.SelectedEntityView);
            }
        });
    }

    private void OnEntitySelected(IUnityEntityView e)
    {
        if (_selector != null && e != null)
        {
            _client.Log.Debug($"Entity Selector Moving to {e}");
            if (e.GameObject == null) return;
            _selector.gameObject.transform.SetParent(e.GameObject.transform, true);
            _selector.gameObject.transform.position = e.GameObject.transform.position;
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
            _selector.gameObject.SetActive(true);
        }
    }
}