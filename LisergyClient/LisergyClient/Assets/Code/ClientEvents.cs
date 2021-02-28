
using Assets.Code.World;
using UnityEngine;

namespace Assets.Code
{
    public delegate void CameraMoveEvent(Vector3 oldPosition, Vector3 newPosition);
    public delegate void ClickTileEvent(ClientTile tile);
    public delegate void SelectPartyEvent(ClientParty party);
    public delegate void PlayerLoginEvent(ClientPlayer player);

    public class ClientEvents
    {
        public static event CameraMoveEvent OnCameraMove;
        public static event ClickTileEvent OnClickTile;
        public static event SelectPartyEvent OnSelectParty;
        public static event PlayerLoginEvent OnPlayerLogin;

        public static void CameraMove(Vector3 oldPosition, Vector3 newPosition)
        {
            OnCameraMove?.Invoke(oldPosition, newPosition);
        }

        public static void ClickTile(ClientTile tile)
        {
            OnClickTile?.Invoke(tile);
        }

        public static void SelectParty(ClientParty party)
        {
            OnSelectParty?.Invoke(party);
        }

        public static void PlayerLogin(ClientPlayer player)
        {
            OnPlayerLogin?.Invoke(player);
        }
    }
}
