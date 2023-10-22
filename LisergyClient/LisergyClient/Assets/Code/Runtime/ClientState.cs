using Game.ECS;
using Game.Tile;
using System;
using UnityEngine;

namespace Assets.Code.Assets.Code.Runtime
{
    /// <summary>
    /// In-memory storage of client UI state
    /// </summary>
    public class ClientStateStorage
    {
        public TileEntity SelectedTile;
        public IEntity SelectedEntity;
        public Vector3 CameraPosition;
    }

    /// <summary>
    /// Client state regarding the UI state. 
    /// This should never handle any logical part of the game just UI
    /// </summary>
    public static class ClientState
    {
        public static event Action<Vector3> OnCameraMove;
        public static event Action<TileEntity> OnSelectTile;
        public static event Action<IEntity> OnSelectEntity;

        private static ClientStateStorage State = new();

        /// <summary>
        /// Tile the player has selected and has highlight on
        /// </summary>
        public static TileEntity SelectedTile { get
            {
                return State.SelectedTile;
            }
            set 
            {
                OnSelectTile?.Invoke(value);
                State.SelectedTile = value;
            }
        }

        /// <summary>
        /// Own party the player is controlling at the moment
        /// </summary>
        public static IEntity SelectedEntity
        {
            get
            {
                return State.SelectedEntity;
            }
            set
            {
                OnSelectEntity?.Invoke(value);
                State.SelectedEntity = value;
            }
        }

        /// <summary>
        /// Camera velocity
        /// </summary>
        public static Vector3 CameraPosition
        {
            get
            {
                return State.CameraPosition;
            }
            set
            {
                OnCameraMove?.Invoke(value);
                State.CameraPosition = value;
            }
        }
    }
}
