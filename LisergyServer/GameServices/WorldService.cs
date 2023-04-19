using Game.Battle;
using Game.Battler;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Player;
using System;

namespace Game.Services
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
            StrategyGame.GlobalGameEvents.Register<OffensiveMoveEvent>(this, OnOffensiveAction);
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
                    tile.SetFlagIncludingChildren(DeltaFlag.SELF_REVEALED);
                }
            }
            else
            {
                var startTile = _world.GetUnusedStartingTile();
                _world.PlaceNewPlayer(ev.Sender, startTile);
                Log.Debug($"New player {ev.Sender.UserID} joined the world");
            }
        }

        [EventMethod]
        public void OnOffensiveAction(OffensiveMoveEvent ev)
        {
            var atk = ev.Attacker as IBattleableEntity;
            var def = ev.Defender as IBattleableEntity;
            if (atk != null && def != null)
            {
                var battleID = Guid.NewGuid();
                var start = new BattleStartPacket(battleID, atk, def);

                _game.NetworkEvents.Call(start);

                if(atk.Owner.CanReceivePackets())
                {
                    atk.Owner.Send(start);
                }
                if(def.Owner.CanReceivePackets())
                {
                    def.Owner.Send(start);
                }
            }
        }
    }
}
