using Game;
using Game.Battle;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.World.Components;
using System;
using System.Collections.Generic;

namespace Game.Listeners
{
    public class WorldService : IEventListener
    {
        private StrategyGame _game;
        private GameWorld _world;

        public WorldService(StrategyGame game)
        {
            _game = game;
            _world = game.World;
            _game.NetworkEvents.Register<JoinWorldPacket>(this, JoinWorld);
            _game.GameEvents.Register<OffensiveMoveEvent>(this, OnOffensiveAction);
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
                {
                    player.Send(tile.UpdatePacket);
                    tile.Components.CallEvent(new TileSentToPlayerEvent(tile, player));
                }
                _world.Game.GameEvents.Call(new PlayerJoinedEvent(player));
            }
            else
            {
                player = ev.Sender;
                var startTile = _world.GetUnusedStartingTile();
                _world.PlaceNewPlayer(player, startTile);
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
                var battleID = Guid.NewGuid();
                _game.NetworkEvents.Call(new BattleStartPacket(battleID, atk, def));
            }
        }

        [EventMethod]
        public void OnEntityMove(EntityMoveInEvent ev)
        {
           
        }

      
    }
}
