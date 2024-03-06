using Assets.Code.Assets.Code.UIScreens.Base;
using Game.Events;
using Game.Engine.Events.Bus;
using Game.Engine.Network;
using Game.Network.ServerPackets;

namespace Assets.Code.Battle
{
    public class BattleListener : IEventListener
    {
        IUiService _screens;

        public BattleListener(EventBus<BasePacket> networkEvents)
        {
            _screens = UnityServicesContainer.Resolve<IUiService>();
            //networkEvents.Register<BattleLogPacket>(this, BattleLog);
            //networkEvents.Register<BattleResultSummaryPacket>(this, BattleSummary);
            //networkEvents.Register<BattleStartPacket>(this, BattleStart);
        }

        /*
        public void BattleLog(BattleLogPacket ev)
        {
            var transition = _screens.Get<TransitionScreen>();
            if(transition != null) transition.CloseTransition(() =>
            {
                var battleScreen = _screens.Get<BattleScreen>();
                if (battleScreen != null)
                {
                    battleScreen.OnFinishedPlayback += () => UIEvents.ReceivedServerBattleFinish(battleScreen.ResultHeader);
                    battleScreen.PlayLog(ev);
                }
            });
        }
        */

        public void BattleSummary(BattleHeaderPacket ev)
        {
            /*
            Log.Info($"Battle Summary Received {ev.BattleHeader.BattleID}");

            var pl = MainBehaviour.LocalPlayer;

            var w = GameView.World;
            var def = w.GetOrCreateClientPlayer(ev.BattleHeader.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.BattleHeader.Attacker.OwnerID);

            // TODO: Remove all this crap and use logic synchronizer
            if (def != null && def.OwnerID != GameId.ZERO)
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

            if (pl.ViewBattles)
            {
                var battleScreen = _screens.Get<BattleScreen>();
                if (battleScreen != null && battleScreen.BattleID == ev.BattleHeader.BattleID)
                {
                    battleScreen.SetResultHeader(ev.BattleHeader);
                }
                MainBehaviour.Networking.Send(new BattleLogRequestPacket()
                {
                    BattleId = ev.BattleHeader.BattleID
                });
            }
            else
            {
                ClientEvents.ReceivedServerBattleFinish(ev.BattleHeader);
            }
            */
        }

        // TODO: Not used
        public void BattleStart(BattleStartPacket ev)
        {
            /*
            Log.Debug("Received battle start");
            var pl = MainBehaviour.LocalPlayer;

            // TODO: Remove all this crap and use logic synchronizer
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

            ClientEvents.ReceivedServerBattleStart(ev.BattleID, ev.Attacker, ev.Defender);
            */
        }
    }
}
