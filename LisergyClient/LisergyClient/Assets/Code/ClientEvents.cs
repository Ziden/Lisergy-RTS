
using Assets.Code.UI;
using Assets.Code.World;
using Game;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public delegate void CameraMoveEvent(Vector3 oldPosition, Vector3 newPosition);
    public delegate void ClickTileEvent(Tile tile);
    public delegate void SelectPartyEvent(ClientParty party);
    public delegate void PlayerLoginEvent(ClientPlayer player);
    public delegate void PartyFinishedMoveEvent(ClientParty party, Tile oldTile, Tile newTile);
    public delegate void StartMovementRequest(ClientParty party, List<Tile> Path);
    public delegate void ActionSelected(ClientParty p, Tile tile, EntityAction action);

    public class ClientEvents
    {
        public static event CameraMoveEvent OnCameraMove;
        public static event ClickTileEvent OnClickTile;
        public static event SelectPartyEvent OnSelectParty;
        public static event PlayerLoginEvent OnPlayerLogin;
        public static event PartyFinishedMoveEvent OnPartyFinishedMove;
        public static event StartMovementRequest OnStartMovementRequest;
        public static event ActionSelected OnActionSelected;

        public static void CameraMove(Vector3 oldPosition, Vector3 newPosition)
        {
            OnCameraMove?.Invoke(oldPosition, newPosition);
        }

        public static void ClickTile(Tile tile)
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

        public static void PartyFinishedMove(ClientParty p, Tile o, Tile n)
        {
            OnPartyFinishedMove?.Invoke(p, o, n);
        }

        public static void StartMovementRequest(ClientParty p, List<Tile> path)
        {
            OnStartMovementRequest?.Invoke(p, path);
        }

        public static void ActionSelected(ClientParty p, Tile tile, EntityAction action)
        {
            OnActionSelected?.Invoke(p, tile, action);
        }
    }
}
