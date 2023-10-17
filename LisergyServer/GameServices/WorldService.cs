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
        }

        /// <summary>
        /// Whenever a player asks to join a game world
        /// </summary>
        public void JoinWorld(JoinWorldPacket ev)
        {
            PlayerEntity player = null;
            if (_world.Players.GetPlayer(ev.Sender.EntityId, out player))
            {
                Log.Debug($"Existing player {player.EntityId} joined");
                foreach (var tile in player.Data.VisibleTiles)
                {
                    tile.SetDeltaFlag(DeltaFlag.SELF_REVEALED);
                }
            }
            else
            {
                ev.Sender.EntityLogic.Player.PlaceNewPlayer(_world.GetUnusedStartingTile());
                Log.Debug($"New player {ev.Sender.EntityId} joined the world");
            }
        }
    }
}
