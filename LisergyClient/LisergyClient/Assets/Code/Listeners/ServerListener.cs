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

        [EventMethod]
        public void BattleFinish(BattleResultEvent ev)
        {
            MainBehaviour.Player.Battles.Add(ev);

            var pl = MainBehaviour.Player;
            var w = _game.GetWorld();
            var def = w.GetOrCreateClientPlayer(ev.BattleHeader.Defender.OwnerID);
            var atk = w.GetOrCreateClientPlayer(ev.BattleHeader.Attacker.OwnerID);

            if (def != null && !Gaia.IsGaia(def.UserID))
            {
                var partyID = ev.BattleHeader.Defender.Units[0].UnitReference.PartyId;
                var party = def.Parties[partyID];
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
        public void BattleStart(BattleStartEvent ev)
        {
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
        public void Message(MessagePopupEvent ev)
        {
            // TODO: Message factory
            if (ev.Type == PopupType.BAD_INPUT)
                UIManager.Notifications.ShowNotification("Path has obstacles");
        }

        [EventMethod]
        public void EntityDestroy(EntityDestroyEvent ev)
        {
            var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.ID);
            if (knownEntity == null)
                throw new System.Exception($"Server sent destroy event for entity {ev.ID} from {ev.OwnerID} at however its not visible to client");

            var obj = knownEntity as IGameObject;
            MainBehaviour.Destroy(obj.GetGameObject());
            if (knownEntity.Tile.StaticEntity == knownEntity)
                knownEntity.Tile.StaticEntity = null;

            if(knownEntity is MovableWorldEntity)
            {
                knownEntity.Tile.MovingEntities.Remove(knownEntity as MovableWorldEntity);

            }

            else if (knownEntity is MovableWorldEntity && knownEntity.Tile.MovingEntities.Contains((MovableWorldEntity)knownEntity))
               
            knownEntity.Tile = null;

        }

        [EventMethod]
        public void EntityMove(EntityMoveEvent ev)
        {
            var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.OwnerID);
            var knownEntity = owner.GetKnownEntity(ev.ID);
            if (knownEntity == null)
                throw new System.Exception($"Server sent move event for entit3y {ev.ID} from {ev.OwnerID} at {ev.X}-{ev.Y} however its not visible to client");

            var newTile = (ClientTile)_game.GetWorld().GetTile(ev.X, ev.Y);
            knownEntity.Tile = newTile;
        }

        [EventMethod]
        public void EntityVisible(EntityVisibleEvent ev)
        {
            var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.Entity.OwnerID);
            var tile = (ClientTile)_game.GetWorld().GetTile(ev.Entity.X, ev.Entity.Y);
            var clientEntity = EntityFactory.InstantiateClientEntity(ev.Entity, owner, tile);
            clientEntity.Tile = tile;
            UIManager.PartyUI.DrawAllParties();
        }

        [EventMethod]
        public void ReceiveTile(TileVisibleEvent ev)
        {
            var newTile = ev.Tile;
            var tile = (ClientTile)_game.GetWorld().GetTile(newTile.X, newTile.Y);
            tile.TileId = ev.Tile.TileId;
            tile.ResourceID = ev.Tile.ResourceID;
            tile.SetVisible(true);
        }

        [EventMethod]
        public void ReceiveSpecs(GameSpecResponse ev)
        {
            if (_game != null)
                throw new System.Exception("Received to register specs twice");
            var world = new ClientWorld(ev);
            _game = new ClientStrategyGame(ev.Spec, world);
        }
    }
}
