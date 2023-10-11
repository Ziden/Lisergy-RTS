
using Assets.Code.UI;
using Assets.Code.World;
using Game.Battle;
using Game.Battle.Data;
using Game.DataTypes;
using Game.Systems.Party;
using Game.Tile;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    /// <summary>
    /// Events fired only internally in client. 
    /// For the client to react to something that happens, these events are the ones that shall be listened to.
    /// </summary>
    public class ClientEvents
    {
        public static event Action<Vector3, Vector3> OnCameraMove;
        public static event Action<TileEntity> OnClickTile;
        public static event Action<PartyEntity> OnSelectParty;
        public static event Action<LocalPlayer> OnPlayerLogin;
        public static event Action<PartyEntity> OnCourseCancelled;
        public static event Action<GameId, BattleTeam, BattleTeam> OnBattleStart;
        public static event Action<BattleHeaderData> OnBattleFinish;
        public static event Action<PartyEntity, TileEntity, TileEntity> OnPartyFinishedMove;
        public static event Action<PartyEntity, List<TileEntity>> OnStartMovementRequest;
        public static event Action<PartyEntity, TileEntity, EntityAction> OnActionSelected;

        public static event Action<PartyView> OnPartyViewUpdated;
        public static event Action<PlayerBuildingView> OnBuildingViewUpdated;
        public static event Action<DungeonView> OnDungeonViewUpdated;
        public static event Action<PartyView> OnPartyViewCreated;
        public static event Action<PlayerBuildingView> OnBuildingViewCreated;
        public static event Action<DungeonView> OnDungeonViewCreated;

        public static void PartyViewUpdated(PartyView view) => OnPartyViewUpdated?.Invoke(view);
        public static void BuildingViewUpdated(PlayerBuildingView view) => OnBuildingViewUpdated?.Invoke(view);
        public static void DungeonViewUpdated(DungeonView view) => OnDungeonViewUpdated?.Invoke(view);

        public static void CameraMove(Vector3 oldPosition, Vector3 newPosition)
        {
            OnCameraMove?.Invoke(oldPosition, newPosition);
        }

        public static void ReceivedServerBattleStart(GameId battleId, BattleTeam attacker, BattleTeam defender)
        {
            OnBattleStart?.Invoke(battleId, attacker, defender);
        }

        public static void ReceivedServerBattleFinish(BattleHeaderData header)
        {
            OnBattleFinish?.Invoke(header);
        }

        public static void CourseCancelled(PartyEntity party)
        {
            OnCourseCancelled?.Invoke(party);
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
