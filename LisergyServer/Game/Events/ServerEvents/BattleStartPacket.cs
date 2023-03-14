using Game.Battle;
using Game.Battles;
using Game.Battles.Actions;
using System;

namespace Game.Events
{
    [Serializable]
    public class BattleStartPacket : ServerEvent
    {
        public ushort X;
        public ushort Y;
        public GameId BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public BattleStartPacket(GameId battleId, IBattleable atk, IBattleable def)
        {
            BattleID = battleId;
            Attacker = atk.GetBattleTeam();
            Defender = def.GetBattleTeam();
            X = def.Tile.X;
            Y = def.Tile.Y;
        }
    }
}
