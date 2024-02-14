using Game.Events.Bus;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Systems.Player;
using Game.World;

namespace Game.Services
{
    public class WorldService : IEventListener
    {
        private GameWorld _world;

        public WorldService(LisergyGame game)
        {
            _world = (GameWorld)game.World;
            game.Network.On<JoinWorldPacket>(JoinWorld);
            game.Network.On<StopEntityPacket>(StopEntity);
        }

        public void StopEntity(StopEntityPacket p)
        {
            var party = p.Sender.Parties[p.PartyIndex];
            if (party.EntityLogic.Harvesting.IsHarvesting())
            {
                party.EntityLogic.Harvesting.StopHarvesting();
            }
        }

        /// <summary>
        /// Whenever a player asks to join a game world
        /// </summary>
        public void JoinWorld(JoinWorldPacket ev)
        {
            PlayerEntity player = null;
            if (_world.Players.GetPlayer(ev.Sender.EntityId, out player))
            {
                _world.Game.Log.Debug($"Existing player {player.EntityId} joined");
                foreach (var pos in player.VisibilityReferences.VisibleTiles)
                {
                    var tile = _world.Map.GetTile(pos.X, pos.Y);
                    tile.SetDeltaFlag(DeltaFlag.SELF_REVEALED);
                }
            }
            else
            {
                ev.Sender.EntityLogic.Player.PlaceNewPlayer(_world.GetUnusedStartingTile());
                _world.Game.Log.Debug($"New player {ev.Sender.EntityId} joined the world");
            }
        }
    }
}
