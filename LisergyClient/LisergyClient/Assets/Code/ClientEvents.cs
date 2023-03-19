
using Assets.Code.UI;
using Assets.Code.World;
using Game;
using Game.Entity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class ClientEvents
    {
        public static event Action<Vector3, Vector3> OnCameraMove;
        public static event Action<Tile> OnClickTile;
        public static event Action<PartyEntity> OnSelectParty;
        public static event Action<ClientPlayer> OnPlayerLogin;
        public static event Action<PartyEntity, Tile, Tile> OnPartyFinishedMove;
        public static event Action<PartyEntity, List<Tile>> OnStartMovementRequest;
        public static event Action<PartyEntity, Tile, EntityAction> OnActionSelected;

        public static void CameraMove(Vector3 oldPosition, Vector3 newPosition)
        {
            OnCameraMove?.Invoke(oldPosition, newPosition);
        }

        public static void ClickTile(Tile tile)
        {
            OnClickTile?.Invoke(tile);
        }

        public static void SelectParty(PartyEntity party)
        {
            OnSelectParty?.Invoke(party);
        }

        public static void PlayerLogin(ClientPlayer player)
        {
            OnPlayerLogin?.Invoke(player);
        }

        public static void PartyFinishedMove(PartyEntity p, Tile o, Tile n)
        {
            OnPartyFinishedMove?.Invoke(p, o, n);
        }

        public static void StartMovementRequest(PartyEntity p, List<Tile> path)
        {
            OnStartMovementRequest?.Invoke(p, path);
        }

        public static void ActionSelected(PartyEntity p, Tile tile, EntityAction action)
        {
            OnActionSelected?.Invoke(p, tile, action);
        }
    }
}
