using Game.Battles;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ClientEvents;
using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.InfiniteDungeon
{
    public class InfiniteDungeonListener : IEventListener
    {

        BlockchainGame _game;

        public InfiniteDungeonListener(BlockchainGame game)
        {
            _game = game;
        }

        private HashSet<string> BattleIDS = new HashSet<string>();

        [EventMethod]
        public void EnterDungeon(EnterInfiniteDungeonPacket p)
        {
            var partyUnits = new Unit[p.UnitIds.Length];
            for (var x = 0; x < p.UnitIds.Length; x++)
                partyUnits[x] = p.Sender.GetUnit(p.UnitIds[x]);

            var invalids = partyUnits.Where(u => u != null && u.InDungeon).ToList();
            if (invalids.Any())
            {
                p.Sender.Send(new MessagePopupPacket(PopupType.BAD_INPUT, "Units are busy" + string.Join(",", invalids)));
                return;
            }
            var battleID = Guid.NewGuid().ToString();
            p.Sender.RunningDungeon = new InfinityDungeon(p.Sender, partyUnits);
            var battleStart = new BattleStartPacket()
            {
                Attacker = new BattleTeam(p.Sender, partyUnits),
                Defender = p.Sender.RunningDungeon.GetNextEnemy(),
                BattleID = battleID
            };
            p.Sender.RunningDungeon.BattleID = battleID;
            var infinityDgPacket = new InfiniteDungeonBattlePacket()
            {
                Level = 1,
                BattleStartPacket = battleStart
            };
            // will be picked up by server implementation to dispatch to player & battle server
            _game.NetworkEvents.Call(infinityDgPacket);
        }
    }
}
