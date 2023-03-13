using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System;

namespace Assets.Code
{
    public class ServerListener : IEventListener
    {
        private ClientStrategyGame _game;

        public ServerListener(EventBus networkEvents)
        {
            networkEvents.Register<BattleResultPacket>(this, BattleFinish);
            networkEvents.Register<BattleStartPacket>(this, BattleStart);
            networkEvents.Register<MessagePopupPacket>(this, Message);
            networkEvents.Register<GameSpecPacket>(this, ReceiveSpecs);
        }

        [EventMethod]
        public void BattleFinish(BattleResultPacket ev)
        {
            Log.Debug("Received battle finish");
            MainBehaviour.Player.Battles.Add(ev);

            var pl = MainBehaviour.Player;
            var w = _game.GetWorld();
            var def = w.GetOrCreateClientPlayer(ev.BattleHeader.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.BattleHeader.Attacker.OwnerID);

            if (def != null && !Gaia.IsGaia(def.UserID))
            {
                var partyID = ev.BattleHeader.Defender.Units[0].UnitReference.PartyId;
                var party = def.GetParty(partyID);
                party.BattleID = null;
            }

            if (atk != null && !Gaia.IsGaia(atk.UserID))
            {
                var partyID = ev.BattleHeader.Attacker.Units[0].UnitReference.PartyId;
                var party = atk.Parties[partyID];
                party.BattleID = null;
            }

            Log.Info("Battle result event");
            UIManager.BattleNotifications.Show(ev.BattleHeader);
        }

        [EventMethod]
        public void BattleStart(BattleStartPacket ev)
        {
            Log.Debug("Received battle startr");
            var pl = MainBehaviour.Player;
            var w = _game.GetWorld();
            var def = w.GetOrCreateClientPlayer(ev.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.Attacker.OwnerID);
            var tile = _game.GetWorld().GetTile(ev.X, ev.Y) as ClientTile;

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

        [EventMethod]
        public void Message(MessagePopupPacket ev)
        {
            // TODO: Message factory
            if (ev.Type == PopupType.BAD_INPUT)
                UIManager.Notifications.ShowNotification("Path has obstacles");
        }

        [EventMethod]
        public void ReceiveSpecs(GameSpecPacket ev)
        {
            Log.Debug("Received specs");
            if (_game != null)
                throw new System.Exception("Received to register specs twice");
            var world = new ClientWorld(ev);
            _game = new ClientStrategyGame(ev.Spec, world);
        }
    }
}
