using Game.Battle;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.World
{
    public class WorldGameListener : IEventListener
    {
        private StrategyGame _game;

        public WorldGameListener(StrategyGame game)
        {
            _game = game;
        }

        [EventMethod]
        public void OnPartyStatusUpdate(PartyStatusUpdateEvent ev)
        {
            ev.Party.Owner.Send(new PartyStatusUpdatePacket(ev.Party));
        }

        [EventMethod]
        public void OnOffensiveAction(OffensiveMoveEvent ev)
        {
            var atk = ev.Attacker as IBattleable;
            var def = ev.Defender as IBattleable;
            if (atk != null && def != null)
            {
                var battleID = Guid.NewGuid().ToString();
                _game.NetworkEvents.Call(new BattleStartPacket()
                {
                    X = ev.Defender.X,
                    Y = ev.Defender.Y,
                    Attacker = atk.GetBattleTeam(),
                    Defender = def.GetBattleTeam(),
                    BattleID = battleID
                });
            }
        }

        [EventMethod]
        public void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            Log.Debug($"Sync {ev.Player.VisibleTiles.Count} visible tiles to " + this);
            foreach (var tile in ev.Player.VisibleTiles)
                SendTileTo(tile, ev.Player);
        }

        [EventMethod]
        public void OnVisibilityChange(PlayerVisibilityChangeEvent ev)
        {
            if (ev.TileVisible)
                SendTileTo(ev.Tile, ev.Viewer.Owner);
        }

        [EventMethod]
        public void OnEntityMove(EntityMoveEvent ev)
        {
            var newTile = ev.NewTile;
            var previousTile = ev.OldTile;

            // changed tile
            var movableEntity = ev.Entity as MovableWorldEntity;
            var allViewers = new HashSet<PlayerEntity>();
            if (previousTile != newTile && movableEntity != null && previousTile != null)
            {
                allViewers.UnionWith(previousTile.PlayersViewing);
                if (newTile != null)
                    allViewers.UnionWith(newTile.PlayersViewing);

                foreach (var viewer in allViewers)
                    viewer.Send(new EntityMovePacket(movableEntity, newTile));
            }

            // Sending Visibility to new viewers
            var newPlayersViewing = new HashSet<PlayerEntity>(newTile.PlayersViewing);
            if (previousTile != null)
                newPlayersViewing.ExceptWith(previousTile.PlayersViewing);

            var packet = new EntityUpdatePacket(ev.Entity);
            foreach (var viewer in newPlayersViewing)
                viewer.Send(packet);
        }

        private void SendTileTo(Tile tile, PlayerEntity player)
        {
            player.Send(new TileVisiblePacket(tile));

            foreach (var movingEntity in tile.MovingEntities)
                player.Send(new EntityUpdatePacket(movingEntity));

            if (tile.StaticEntity != null)
                player.Send(new EntityUpdatePacket(tile.StaticEntity));
        }
    }
}
