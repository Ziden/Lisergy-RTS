using Game.Battles;
using Game.Battles.Actions;
using Game.DataTypes;
using Game.Entity.Logic;
using System;

namespace Game.Events
{
    [Serializable]
    public class BattleStartPacket : ServerPacket
    {
        public ushort X;
        public ushort Y;
        public GameId BattleID;
        public BattleTeam Attacker;
        public BattleTeam Defender;

        public BattleStartPacket(GameId battleId, IBattleableEntity atk, IBattleableEntity def)
        {
            BattleID = battleId;
            Attacker = atk.BattleLogic.GetBattleTeam();
            Defender = def.BattleLogic.GetBattleTeam();
            X = def.Tile.X;
            Y = def.Tile.Y;
        }
    }
}
