using System;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Godot;

namespace LisergyGodotClient.Src.Services
{
	namespace LisergyGodotClient.Src.Controllers
	{
		public interface IInputService
		{
			void CenterCameraOn(IEntity entity);
		}

		public class GodotCameraInputService : IInputService, IEventListener
		{
			private readonly Camera3D _camera;

			private readonly IClientSdk _sdk;
			private readonly IClientStateService _state;
			private Vector3 _cameraDelta;
			private Vector3 _cameraPosition;
			private bool _isDragging;
			private DateTime _lastDown;
			private Vector2 _lastMousePosition;
			private DateTime _lastUp;

			public GodotCameraInputService(Camera3D camera, IClientSdk sdk, IClientStateService state)
			{
				_state = state;
				_camera = camera;
				_sdk = sdk;
				_isDragging = false;
				_cameraPosition = _cameraDelta = new Vector3(10, 30, 10);
				_sdk.ClientEvents.On<GameStartedEvent>(this, OnGameStart);
				_state.SelectedParty.OnChanged += CenterCameraOn;
			}

			public void CenterCameraOn(IEntity entity)
			{
				if (entity == null) return;

				var godotObject = entity.GetView().GameObject;
				var objectPosition = godotObject.Location.ToGodotVector3();
				GD.Print(objectPosition);
				var currentHeight = _cameraPosition.Y;

				_cameraPosition = new Vector3(
					objectPosition.X + 16,
					currentHeight,
					objectPosition.Z + 18
				);

				UpdateCameraTransform();
				_state.CameraPosition.Value = _cameraPosition;
			}

			private void OnGameStart(GameStartedEvent ev)
			{
				UpdateCameraTransform();
			}

			private Vector3? IntersectsGroundPlane(Vector3 from, Vector3 to)
			{
				var groundPlane = new Plane(Vector3.Up, 0);
				return groundPlane.IntersectsRay(from, to);
			}

			private void UpdateCameraTransform()
			{
				_camera.GlobalTransform = new Transform3D(_camera.GlobalTransform.Basis, _cameraPosition);
			}

			public void ReceiveClickDown(Vector2 mousePosition)
			{
				_isDragging = true;
				_lastMousePosition = mousePosition;
				_lastDown = DateTime.Now;
			}

			public Vector2 GetClickedTile(Vector2 position)
			{
				var from = _camera.ProjectRayOrigin(position);
				var to = from + _camera.ProjectRayNormal(position) * 1000;
				var intersection = IntersectsGroundPlane(from, to);
				if (intersection.HasValue)
					return new Vector2(Mathf.Round(intersection.Value.X), Mathf.Round(intersection.Value.Z));
				return default;
			}

			public void ReceiveClickUp(Vector2 position)
			{
				_isDragging = false;
				_lastUp = DateTime.Now;
				if (_lastUp - _lastDown < AssetConfigs.TAP_TIME) _state.ReceiveTapInput(GetClickedTile(position));
			}

			public void ReceiveDrag(Vector2 currentMousePosition)
			{
				if (_isDragging)
				{
					var delta = currentMousePosition - _lastMousePosition;
					_lastMousePosition = currentMousePosition;

					var inverseBasis = _camera.Basis.Inverse();
					var right = inverseBasis.Row0;
					var forward = -inverseBasis.Row1;

					var old = _cameraPosition;

					_cameraPosition -= right * delta.X * 0.015f;
					_cameraPosition -= forward * delta.Y * 0.015f;
					_cameraPosition.Y = old.Y;
					if (old != _cameraPosition) ClientServices.State.CameraPosition.Value = _cameraPosition;
					UpdateCameraTransform();
				}
			}
		}
	}
}