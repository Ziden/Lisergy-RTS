
using Game;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public delegate void CameraMoveEvent(Vector3 oldPosition, Vector3 newPosition);
  
    public delegate void PlayerLoginEvent(ClientPlayer player);

    public class ClientEvents
    {
        public static event CameraMoveEvent OnCameraMove;
        public static event PlayerLoginEvent OnPlayerLogin;
      
        public static void CameraMove(Vector3 oldPosition, Vector3 newPosition)
        {
            OnCameraMove?.Invoke(oldPosition, newPosition);
        }

        public static void PlayerLogin(ClientPlayer player)
        {
            OnPlayerLogin?.Invoke(player);
        }
    }
}