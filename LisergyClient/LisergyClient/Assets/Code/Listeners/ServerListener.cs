using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;
using System;

namespace Assets.Code
{
    public class ServerListener
    {
        private ClientStrategyGame _game;

        public ServerListener()
        {
            NetworkEvents.OnSpecResponse += ReceiveSpecs;
        }

        public void RegisterGameListeners()
        {
            NetworkEvents.OnBattleResult += BattleFinish;
            NetworkEvents.OnBattleStart += BattleStart;
            NetworkEvents.OnTileVisible += ReceiveTile;
            NetworkEvents.OnEntityVisible += EntityVisible;
            NetworkEvents.OnEntityMove += EntityMove;
            NetworkEvents.OnMessagePopup += Message;
        }

        public void BattleFinish(BattleResultEvent ev)
        {
            // Delay the receiving of it so battles have delays
            Awaiter.WaitFor(TimeSpan.FromSeconds(3), () =>
            {
                var pl = MainBehaviour.Player;
                var w = _game.GetWorld();
                var def = w.GetOrCreateClientPlayer(ev.BattleHeader.Defender.OwnerID);
                var atk = w.GetOrCreateClientPlayer(ev.BattleHeader.Attacker.OwnerID);

                if (def != null)
                {
                    var partyID = ev.BattleHeader.Defender.Units[0].UnitReference.PartyId;
                    var party = def.Parties[partyID];
                    party.BattleID = null;
                }

                if (atk != null)
                {
                    var partyID = ev.BattleHeader.Attacker.Units[0].UnitReference.PartyId;
                    var party = atk.Parties[partyID];
                    party.BattleID = null;
                }

            });
        }

        public void BattleStart(BattleStartEvent ev)
        {
            var pl = MainBehaviour.Player;
            var w = _game.GetWorld();
            var def = w.GetOrCreateClientPlayer(ev.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.Attacker.OwnerID);
            var tile = _game.GetWorld().GetTile(ev.X, ev.Y) as ClientTile;

            if (def != null)
            {
                var partyID = ev.Defender.Units[0].UnitReference.PartyId;
                var party = def.Parties[partyID];
                party.Tile = tile;
                party.BattleID = ev.BattleID;
                //def.BattlesStarts[ev.BattleID] = ev;
            }

            if (atk != null)
            {
                var partyID = ev.Attacker.Units[0].UnitReference.PartyId;
                var party = atk.Parties[partyID];
                party.Tile = tile;
                party.BattleID = ev.BattleID;
                //atk.BattlesStarts[ev.BattleID] = ev;
            }
        }


        public void Message(MessagePopupEvent ev)
        {
            // TODO: Message factory
            if (ev.Type == PopupType.BAD_INPUT)
                UIManager.Notifications.ShowNotification("Path has obstacles");
        }

        public void EntityMove(EntityMoveEvent ev)
        {
            var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.ID);
            if (knownEntity == null)
                throw new System.Exception($"Server sent move event for entity {ev.ID} from {ev.OwnerID} at {ev.X}-{ev.Y} however its not visible to client");
            var newTile = (ClientTile)_game.GetWorld().GetTile(ev.X, ev.Y);
            knownEntity.Tile = newTile;
        }

        public void EntityVisible(EntityVisibleEvent ev)
        {
            var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.Entity.OwnerID);
            var tile = (ClientTile)_game.GetWorld().GetTile(ev.Entity.X, ev.Entity.Y);
            var clientEntity = EntityFactory.InstantiateClientEntity(ev.Entity, owner, tile);
            clientEntity.Tile = tile;
            UIManager.PartyUI.DrawAllParties();
        }

        public void ReceiveTile(TileVisibleEvent ev)
        {
            var newTile = ev.Tile;
            var tile = (ClientTile)_game.GetWorld().GetTile(newTile.X, newTile.Y);
            tile.TileId = ev.Tile.TileId;
            tile.ResourceID = ev.Tile.ResourceID;
            tile.SetVisible(true);
        }

        public void ReceiveSpecs(GameSpecResponse ev)
        {
            if (_game != null)
                throw new System.Exception("Received to register specs twice");
            var world = new ClientWorld(ev);
            _game = new ClientStrategyGame(ev.Spec, world);
            RegisterGameListeners();
        }
    }
}
