using Game.ECS;
using Game.Events;
using Game.Events.GameEvents;
using Game.Network;
using Game.Network.ServerPackets;
using Game.Systems.FogOfWar;
using Game.Systems.Tile;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Game.Systems.Player
{
    public class PlayerSystem : LogicSystem<PlayerComponent, PlayerLogic>
    {
        public PlayerSystem(LisergyGame game) : base(game)
        {
        }

        public override void OnEnabled()
        {
            Game.Network.On<BattleResultPacket>(OnBattleResultPacket);
            EntityEvents.On<TileVisibilityChangedForPlayerEvent>(OnTileVisibilityChanged);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnTileVisibilityChanged(IEntity player, TileVisibilityChangedForPlayerEvent ev)
        {
            var tileObj = ev.Tile;
           
            var owner = Players.GetPlayer(ev.Explorer.OwnerID);
            if (ev.Visible)
            {
                owner.Data.OnceExplored.Add(tileObj);
                owner.Data.VisibleTiles.Add(tileObj);
            }
            else
            {
                owner.Data.VisibleTiles.Remove(tileObj);
            }
        }
    }
}
