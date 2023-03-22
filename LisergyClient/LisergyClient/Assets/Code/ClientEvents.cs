
using Assets.Code.UI;
using Game.Party;
using Game.Tile;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class ClientEvents
    {
        public static event Action<Vector3, Vector3> OnCameraMove;
        public static event Action<TileEntity> OnClickTile;
        public static event Action<PartyEntity> OnSelectParty;
        public static event Action<LocalPlayer> OnPlayerLogin;
        public static event Action<PartyEntity, TileEntity, TileEntity> OnPartyFinishedMove;
        public static event Action<PartyEntity, List<TileEntity>> OnStartMovementRequest;
        public static event Action<PartyEntity, TileEntity, EntityAction> OnActionSelected;

        public static void CameraMove(Vector3 oldPosition, Vector3 newPosition)
        {
            OnCameraMove?.Invoke(oldPosition, newPosition);
        }

        public static void ClickTile(TileEntity tile)
        {
            OnClickTile?.Invoke(tile);
        }

        public static void SelectParty(PartyEntity party)
        {
            OnSelectParty?.Invoke(party);
        }

        public static void PlayerLogin(LocalPlayer player)
        {
            OnPlayerLogin?.Invoke(player);
        }

        public static void PartyFinishedMove(PartyEntity p, TileEntity o, TileEntity n)
        {
            OnPartyFinishedMove?.Invoke(p, o, n);
        }

        public static void StartMovementRequest(PartyEntity p, List<TileEntity> path)
        {
            OnStartMovementRequest?.Invoke(p, path);
        }

        public static void ActionSelected(PartyEntity p, TileEntity tile, EntityAction action)
        {
            OnActionSelected?.Invoke(p, tile, action);
        }
    }
}
