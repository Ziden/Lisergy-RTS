using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Battle
{
    public class BattleListener : IEventListener
    {
        public BattleListener(EventBus<ServerPacket> networkEvents)
        {
            networkEvents.Register<BattleResultPacket>(this, BattleFinish);
            networkEvents.Register<BattleStartPacket>(this, BattleStart);
        }

        [EventMethod]
        public void BattleFinish(BattleResultPacket ev)
        {
            Log.Debug("Received battle finish");
            MainBehaviour.Player.Battles.Add(ev);

            var pl = MainBehaviour.Player;
            var w = ClientStrategyGame.ClientWorld;
            var def = w.GetOrCreateClientPlayer(ev.BattleHeader.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.BattleHeader.Attacker.OwnerID);

            if (def != null && !Gaia.IsGaia(def.UserID))
            {
                var partyID = ev.BattleHeader.Defender.Units[0].UnitReference.PartyId;
                var party = def.GetParty(partyID);
                party.BattleID = GameId.ZERO;
            }

            if (atk != null && !Gaia.IsGaia(atk.UserID))
            {
                var partyID = ev.BattleHeader.Attacker.Units[0].UnitReference.PartyId;
                var party = atk.Parties[partyID];
                party.BattleID = GameId.ZERO;
            }

            Log.Info("Battle result event");
            UIManager.BattleNotifications.Show(ev.BattleHeader);
        }

        [EventMethod]
        public void BattleStart(BattleStartPacket ev)
        {
            Log.Debug("Received battle startr");
            var pl = MainBehaviour.Player;
            var w = ClientStrategyGame.ClientWorld;
            var def = w.GetOrCreateClientPlayer(ev.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.Attacker.OwnerID);
            var tile = w.GetClientTile(ev.X, ev.Y);

            if (def != null && !Gaia.IsGaia(def.UserID))
            {
                var partyID = ev.Defender.Units[0].UnitReference.PartyId;
                var party = def.Parties[partyID];
                party.Tile = tile;
                party.BattleID = ev.BattleID;
            }

            if (atk != null && !Gaia.IsGaia(atk.UserID))
            {
                var partyID = ev.Attacker.Units[0].UnitReference.PartyId;
                var party = atk.Parties[partyID];
                party.Tile = tile;
                party.BattleID = ev.BattleID;
            }
        }
    }
}
