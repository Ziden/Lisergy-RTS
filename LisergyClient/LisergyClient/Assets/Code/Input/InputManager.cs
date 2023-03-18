using System;
using Assets.Code.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Code
{
    public interface IInputManager
    {
        public delegate void OnClickTileHandler(ClientTile tile);

        public OnClickTileHandler OnClickTile { get; set; }
        
    }

    public class InputManager : MonoBehaviour, IInputManager
    {

        
        public IInputManager.OnClickTileHandler OnClickTile { get; set; }

        private GameInputs _actions;
        
        private void OnEnable()
        {
            _actions = new GameInputs();
            _actions.Enable();
            _actions.Game.ClickOnScreen.performed += context =>
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                var a = context.ReadValue<Vector2>();
                Debug.Log(a.ToString());

                Debug.Log("Old:"+Input.mousePosition+"    New:"+a.ToString());
                var ray = Camera.main.ScreenPointToRay(new Vector3(a.x, a.y));
                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.collider == null)
                    {
                        return;
                    }
                    
                    Debug.Log("Hit", hit.collider);

                    var tileComponent = hit.collider.GetComponentInParent<TileRandomizerBehaviour>();
                    if (tileComponent == null)
                        return;
                    var tile = (ClientTile)tileComponent.Tile;
                    OnClickTile?.Invoke(tile);
                }
            };
        }


        private void OnDisable()
        {
            _actions.Disable();
        }

    }
}