using Game.Battle;
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
            IBattleable atk = ev.Attacker as IBattleable;
            IBattleable def = ev.Defender as IBattleable;   
            if(atk != null && def != null)
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

            if(ev.Tile.StaticEntity != null || ev.Tile.MovingEntities.Count > 0)
            {
                var asd = 123;
            }
        }

        [EventMethod]
        public void OnEntityMove(EntityMoveEvent ev)
        {
            var newTile = ev.NewTile;
            var previousTile = ev.OldTile;

            HashSet<WorldEntity> oldViewers = null;
            if (previousTile != null)
                oldViewers = previousTile.EntitiesViewing;

            var newViewers = new HashSet<WorldEntity>(newTile.EntitiesViewing);
            if (oldViewers != null)
                newViewers.ExceptWith(oldViewers);

            HashSet<PlayerEntity> playerViewers = new HashSet<PlayerEntity>(newViewers.Select(v => v.Owner));
            var packet = new EntityVisiblePacket(ev.Entity);
            foreach (var viewer in newTile.EntitiesViewing)
                if (playerViewers.Remove(viewer.Owner))
                    viewer.Owner.Send(packet);
        }

        private void SendTileTo(Tile tile, PlayerEntity player)
        {
            player.Send(new TileVisiblePacket(tile));

            foreach (var movingEntity in tile.MovingEntities)
                player.Send(new EntityVisiblePacket(movingEntity));

            if (tile.StaticEntity != null)
                player.Send(new EntityVisiblePacket(tile.StaticEntity));
        }
    }
}
