using Game;
using Game.Events.Bus;
using Game.Events.ServerEvents;

namespace Assets.Code.Login
{
    public class DungeonPacketListener : IEventListener
    {
        [EventMethod]
        public void OnBattle(InfiniteDungeonBattlePacket ev)
        {
            Log.Debug($"Received infinite dungeon battle {ev.BattleStartPacket.Attacker} vs {ev.BattleStartPacket.Defender}");
        }
    }
}
