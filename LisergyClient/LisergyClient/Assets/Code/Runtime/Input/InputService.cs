using ClientSDK.Data;
using Game.Tile;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public interface IInputService : IGameService
    {
    }

    public class InputService : MonoBehaviour, IInputService
    {
        private GameInputs _actions;
        private Camera _cam;
        private CameraBehaviour _gameCamera;


        private void OnEnable()
        {
            _actions = new GameInputs();
            _actions.Enable();
            _actions.UI.Enable();
            _cam = Camera.main;
            _gameCamera = _cam.GetComponent<CameraBehaviour>(); 
        }

        private void Update()
        {
            OnCameraUpdate();
            OnClickUpdate();
            OnCameraZoomUpdate();
        }

        private void OnClickUpdate()
        {
            if (!_actions.Game.ClickOnScreen.WasPerformedThisFrame()) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var a = _actions.Game.ClickOnScreen.ReadValue<Vector2>();
            var ray = _cam.ScreenPointToRay(new Vector3(a.x, a.y));
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider == null) return;
                var tileComponent = hit.collider.GetComponentInParent<TileMonoComponent>();
                if (tileComponent == null) return;
                var tile = tileComponent.Tile;
                UIEvents.ClickTile(tile);
            }
        }

        private void OnCameraUpdate()
        {
            var camera = _actions.Game.CameraMovement;
            var velocity = camera.ReadValue<Vector2>();
            if (_actions.Game.CameraSpeedDown.IsPressed()) velocity *= 0.5f;
            else if (_actions.Game.CameraSpeedUp.IsPressed()) velocity *= 1.5f;
            if (velocity.magnitude == 0) return;
            _gameCamera.MoveCamera(velocity);
        }

        public void OnSceneLoaded() { }
        public void OnCameraZoomUpdate() { }
        private void OnDisable() { _actions.Disable(); }
    }
}