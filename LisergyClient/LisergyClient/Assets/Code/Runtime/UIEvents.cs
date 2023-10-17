
using Assets.Code.UI;
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
    /// Contains all UI hooks for all UI events of the game
    /// </summary>
    public class UIEvents
    {
        public static event Action<Vector3> OnCameraMove;
        public static event Action<TileEntity> OnClickTile;
        public static event Action<PartyEntity> OnSelectParty;
        public static event Action<PartyEntity> OnCourseCancelled;
        public static event Action<GameId, BattleTeam, BattleTeam> OnBattleStart;
        public static event Action<BattleHeaderData> OnBattleFinish;
        public static event Action<PartyEntity, TileEntity, EntityAction> OnActionSelected;

        public static void CameraMoved(Vector3 newPosition)
        {
            OnCameraMove?.Invoke(newPosition);
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

        public static void ActionSelected(PartyEntity p, TileEntity tile, EntityAction action)
        {
            OnActionSelected?.Invoke(p, tile, action);
        }
    }
}
