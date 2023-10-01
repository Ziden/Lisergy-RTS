using Game.Battle;
using Game.DataTypes;
using Game.Events;
using Game.Systems.Battler;
using System;

namespace Game.Network.ServerPackets
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
            Attacker = atk.BattleGroupLogic.GetBattleTeam();
            Defender = def.BattleGroupLogic.GetBattleTeam();
            X = def.Tile.X;
            Y = def.Tile.Y;
        }
    }
}
