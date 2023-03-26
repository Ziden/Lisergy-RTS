using System;
using Assets.Code.Code;
using Assets.Code.World;
using Game.Tile;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Code
{
    public interface IInputManager
    {
        public delegate void OnClickTileHandler(TileEntity tile);
        public delegate void OnCameraMoveHandler(Vector2 velocity);
        public delegate void OnCameraZoomHandler(float velocity);

        public OnClickTileHandler OnClickTile { get; set; }
        public OnCameraMoveHandler OnCameraMove { get; set; }
        
        public  OnCameraZoomHandler OnCameraZoom { get; set; }
    }

    public class InputManager : MonoBehaviour, IInputManager
    {
        public IInputManager.OnClickTileHandler OnClickTile { get; set; }
        public IInputManager.OnCameraMoveHandler OnCameraMove { get; set; }
        public IInputManager.OnCameraZoomHandler OnCameraZoom { get; set; }

        private GameInputs _actions;

        private void OnEnable()
        {
            _actions = new GameInputs();
            _actions.Enable();
            _actions.UI.Enable();
        }

        private void Update()
        {
            OnCameraUpdate();
            OnClickUpdate();
            OnCameraZoomUpdate();
        }

        public void OnCameraZoomUpdate()
        {
            // SCROLL IS BROKEN AT THE CURRENT VERSION :P
        }
        private void OnClickUpdate()
        {
            var act = _actions.Game.ClickOnScreen;
            if (!act.WasPerformedThisFrame()) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var a = _actions.Game.ClickOnScreen.ReadValue<Vector2>();
            Debug.Log(a.ToString());

            var ray = Camera.main.ScreenPointToRay(new Vector3(a.x, a.y));
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider == null)
                {
                    return;
                }

                Debug.Log("Hit", hit.collider);

                var tileComponent = hit.collider.GetComponentInParent<TileMonoComponent>();
                if (tileComponent == null) return;
                var tile = tileComponent.Tile;
                OnClickTile?.Invoke(tile);
            }
        }

        private void OnCameraUpdate()
        {
            var camera = _actions.Game.CameraMovement;
            var velocity = camera.ReadValue<Vector2>();

            if (_actions.Game.CameraSpeedDown.IsPressed())
            {
                velocity *= 0.5f;
            }
            else if (_actions.Game.CameraSpeedUp.IsPressed())
            {
                velocity *= 1.5f;
            }

            if (velocity.magnitude == 0) return;
            OnCameraMove?.Invoke(velocity);
        }


        private void OnDisable()
        {
            _actions.Disable();
        }
    }
}