﻿using Game.Battle.Data;
using Game.DataTypes;
using System;

namespace Game.Battle.BattleActions
{
    [Serializable]
    public class AttackAction : BattleAction
    {
        public GameId DefenderID;

        [NonSerialized]
        private BattleUnit _defender;

        public BattleUnit Defender
        {
            get
            {
                if (_defender == null)
                {
                    _defender = Battle.FindBattleUnit(DefenderID);
                }

                return _defender;
            }
            set { _defender = value; DefenderID = value.UnitID; }
        }

        public AttackAction(TurnBattle battle, BattleUnit atk, BattleUnit def) : base(battle, atk)
        {
            Defender = def;
        }

        public override string ToString()
        {
            return $"<Attack From={UnitID} To={DefenderID}>";
        }
    }
}