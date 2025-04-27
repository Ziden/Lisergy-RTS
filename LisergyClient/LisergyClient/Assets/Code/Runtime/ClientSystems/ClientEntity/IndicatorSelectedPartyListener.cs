using Assets.Code;
using Assets.Code.Assets.Code.Runtime;
using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Engine.Events.Bus;
using Game.Entities;
using GameAssets;
using UnityEngine;

/// <summary>
/// Hook responsible for controlling the entity selector icon that shows which entity the local player have selected
/// </summary>
public class IndicatorSelectedPartyListener : IEventListener
{
    private IClientSdk _client;
    private EntitySelectionMonoBehaviour _selector;
    private IGameObject _selectorGameObject; 
    
    public IndicatorSelectedPartyListener(IClientSdk client)
    {
        _client = client;
        ClientViewState.OnSelectEntity += OnEntitySelected;
        _client.UnityServices().Assets.CreateMapObject(MapObjectPrefab.UnitCursor, Vector3.zero, Quaternion.identity).ContinueWith(o =>
        {
            _selector = o.GetComponent<EntitySelectionMonoBehaviour>();
            _selectorGameObject = _selector.ToLisergyGameObject();
            o.SetActive(false);
            if (ClientViewState.SelectedEntityView != null)
            {
                OnEntitySelected(ClientViewState.SelectedEntityView);
            }
        }).Forget();
    }

    private void OnEntitySelected(IUnityEntityView e)
    {
        if (_selector != null && e != null)
        {
            if (e.GameObject == null) return;

            e.GameObject.AddChild(_selectorGameObject);
            _selectorGameObject.Location = e.GameObject.Location;
            
            if (e.EntityType == EntityType.Building) // TODO: Move this to other place
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