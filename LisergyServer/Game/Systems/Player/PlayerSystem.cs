using Game.ECS;
using Game.Network.ServerPackets;

namespace Game.Systems.Player
{
    public class PlayerSystem : LogicSystem<PlayerDataComponent, PlayerLogic>
    {
        public PlayerSystem(LisergyGame game) : base(game)
        {
        }

        public override void OnEnabled()
        {
            Game.Network.On<BattleResultPacket>(OnBattleResultPacket);
        }

        /// <summary>
        /// Received from battle server
        /// </summary>
        private void OnBattleResultPacket(BattleResultPacket packet) 
        {
            var atkPlayer = Game.Players[packet.Header.Attacker.OwnerID];
            var defPlayer = Game.Players[packet.Header.Defender.OwnerID];

            if (atkPlayer != null) GetLogic(atkPlayer).RecordHeader(packet.Header);
            if (defPlayer != null) GetLogic(defPlayer).RecordHeader(packet.Header);
        }
    }
}
