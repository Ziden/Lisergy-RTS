using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using LisergyMessageQueue;

namespace GameServer
{
    public class GameServerListener : IEventListener
    {
        private BlockchainGame game;

        public GameServerListener(BlockchainGame game)
        {
            this.game = game;
        }

        [EventMethod]
        public void JoinWorld(JoinWorldPacket p)
        {
            // informing battle server player is joined and need to know about his battles
            if (p.Sender.RunningDungeon != null)
            {
                EventMQ.Send("battles", new BattleRefreshPacket(
                    p.Sender.RunningDungeon.BattleID,
                    p.Sender.UserID
                ));
            }
        }

        [EventMethod]
        public void OnBattleStart(InfiniteDungeonBattlePacket packet)
        {
            Log.Debug($"Routing {packet}");
            var p = packet.BattleStartPacket;

            // sending packet to attacker/defender
            p.Attacker.Owner?.Send(packet);
            p.Defender.Owner?.Send(packet);

            // Sending to battle server
            EventMQ.Send("battles", p);
        }
    }
}
