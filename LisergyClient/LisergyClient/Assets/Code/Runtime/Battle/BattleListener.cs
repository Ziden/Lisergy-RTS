using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.World;
using Game;
using Game.DataTypes;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Network.ServerPackets;
using Game.Player;
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
            networkEvents.Register<BattleResultSummaryPacket>(this, BattleSummary);
            networkEvents.Register<BattleResultPacket>(this, BattleFinish);
            networkEvents.Register<BattleStartPacket>(this, BattleStart);
        }

        [EventMethod]
        public void BattleSummary(BattleResultSummaryPacket ev)
        {
            Log.Info($"Battle Summary Received {ev.BattleHeader.BattleID}");
            var pl = MainBehaviour.LocalPlayer;
            var w = GameView.World;
            var def = w.GetOrCreateClientPlayer(ev.BattleHeader.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.BattleHeader.Attacker.OwnerID);

            // TODO: Remove all this crap and use logic synchronizer
            if (def != null && !Gaia.IsGaia(def.UserID))
            {
                var partyID = ev.BattleHeader.Defender.Units[0].UnitReference.PartyId;
                var party = def.GetParty(partyID);
                party.BattleGroupLogic.BattleID = GameId.ZERO;
            }

            if (atk != null && !Gaia.IsGaia(atk.UserID))
            {
                var partyID = ev.BattleHeader.Attacker.Units[0].UnitReference.PartyId;
                var party = atk.Parties[partyID];
                party.BattleGroupLogic.BattleID = GameId.ZERO;
            }

            Log.Info("Battle result event");
            ServiceContainer.Resolve<IScreenService>().Open<BattleNotificationScreen, BattleNotificationSetup>(new BattleNotificationSetup()
            {
                BattleHeader = ev.BattleHeader
            });
        }

        [EventMethod]
        public void BattleFinish(BattleResultPacket ev)
        {
            Log.Info($"Battle Finish Received {ev.FinalStateHeader.BattleID}");
            //MainBehaviour.LocalPlayer.Battles.Add(ev);
        }

        [EventMethod]
        public void BattleStart(BattleStartPacket ev)
        {
            Log.Debug("Received battle startr");

            // TODO: Remove all this crap and use logic synchronizer
            var pl = MainBehaviour.LocalPlayer;
            var w = GameView.World;
            var def = w.GetOrCreateClientPlayer(ev.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.Attacker.OwnerID);
            var tile = w.GetTile(ev.X, ev.Y);

            if (def != null && !Gaia.IsGaia(def.UserID))
            {
                var partyID = ev.Defender.Units[0].UnitReference.PartyId;
                var party = def.Parties[partyID];
                party.Tile = tile;
                party.BattleGroupLogic.BattleID = ev.BattleID;
            }

            if (atk != null && !Gaia.IsGaia(atk.UserID))
            {
                var partyID = ev.Attacker.Units[0].UnitReference.PartyId;
                var party = atk.Parties[partyID];
                party.Tile = tile;
                party.BattleGroupLogic.BattleID = ev.BattleID;
            }
        }
    }
}
