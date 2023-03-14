using Game;
using Game.Battle;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Listeners
{
    public class WorldService : IEventListener
    {
        private StrategyGame _game;
        private GameWorld _world;

        private HashSet<PlayerEntity> viewersCache = new HashSet<PlayerEntity>();

        public WorldService(StrategyGame game)
        {
            _game = game;
            _world = game.World;
            _game.NetworkEvents.Register<JoinWorldPacket>(this, JoinWorld);
            _game.GameEvents.Register<OffensiveMoveEvent>(this, OnOffensiveAction);
            _game.GameEvents.Register<PlayerVisibilityChangeEvent>(this, OnVisibilityChange);
            _game.GameEvents.Register<EntityMoveInEvent>(this, OnEntityMove);
        }

        [EventMethod]
        public void JoinWorld(JoinWorldPacket ev)
        {
            PlayerEntity player = null;
            if (_world.Players.GetPlayer(ev.Sender.UserID, out player))
            {
                Log.Debug($"Existing player {player.UserID} joined");
                foreach (var tile in player.VisibleTiles)
                    SendTileTo(tile, player);
                _world.Game.GameEvents.Call(new PlayerJoinedEvent(player));
            }
            else
            {
                player = ev.Sender;
                _world.PlaceNewPlayer(player);
                Log.Debug($"New player {player.UserID} joined the world");
            }
        }

        [EventMethod]
        public void OnOffensiveAction(OffensiveMoveEvent ev)
        {
            var atk = ev.Attacker as IBattleable;
            var def = ev.Defender as IBattleable;
            if (atk != null && def != null)
            {
                var battleID = Guid.NewGuid().ToString();
                _game.NetworkEvents.Call(new BattleStartPacket(battleID, atk, def));
            }
        }

        [EventMethod]
        public void OnVisibilityChange(PlayerVisibilityChangeEvent ev)
        {
            if (ev.TileVisible)
                SendTileTo(ev.Tile, ev.Viewer.Owner);
        }

        [EventMethod]
        public void OnEntityMove(EntityMoveInEvent ev)
        {
            var newTile = ev.ToTile;
            var previousTile = ev.FromTile;

            var movableEntity = ev.Entity as MovableWorldEntity;
            viewersCache.Clear();
            var allViewers = viewersCache;
            if (previousTile != newTile && movableEntity != null && previousTile != null)
            {
                allViewers.UnionWith(previousTile.GetComponent<TileVisibilityComponent>().PlayersViewing);
                if (newTile != null)
                    allViewers.UnionWith(newTile.GetComponent<TileVisibilityComponent>().PlayersViewing);

                var movePacket = new EntityMovePacket(movableEntity, newTile);
                foreach (var viewer in allViewers)
                    viewer.Send(movePacket);
            }

            // Sending Visibility to new viewers
            var newPlayersViewing = new HashSet<PlayerEntity>(newTile.GetComponent<TileVisibilityComponent>().PlayersViewing);
            if (previousTile != null)
                newPlayersViewing.ExceptWith(previousTile.GetComponent<TileVisibilityComponent>().PlayersViewing);

            var packet = new EntityUpdatePacket(ev.Entity);
            foreach (var viewer in newPlayersViewing)
                viewer.Send(packet);
        }

        public void SendTileTo(Tile tile, PlayerEntity player)
        {
            player.Send(new TileUpdatePacket(tile));
            tile.CallEvent(new TileSentToPlayerEvent(tile, player));
        }
    }
}
