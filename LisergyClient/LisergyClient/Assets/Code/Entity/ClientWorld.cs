using Game;
using Game.Entity;
using Game.Events.ServerEvents;
using Game.World;
using System.Collections.Generic;

namespace Assets.Code.World
{
    public class ClientWorld : GameWorld
    {
        public ClientWorld(GameSpecPacket gameSpecs): base(int.MaxValue, gameSpecs.WorldX, gameSpecs.WorldY)
        {
        }

        public Dictionary<string, Party> Parties = new Dictionary<string, Party>();

        public ClientTile GetClientTile(int tileX, int tileY)
        {
            return Map.GetTile(tileX, tileY) as ClientTile;
        }

        public ClientTile GetClientTile(WorldEntity e)
        {
            return Map.GetTile(e.X, e.Y) as ClientTile;
        }

        public ClientPlayer GetOrCreateClientPlayer(string uid)
        {
            if (uid == null)
                return null;

            PlayerEntity pl;
            if (Players.GetPlayer(uid, out pl))
                return (ClientPlayer)pl;
            if (uid == MainBehaviour.Player.UserID)
            {
                Players.Add(MainBehaviour.Player);
                return MainBehaviour.Player;
            }
            pl = new ClientPlayer();
            pl.UserID = uid;
            Players.Add(pl);
            return (ClientPlayer)pl;
        }

        // In client we generate the tiles on-demand
        public override void CreateMap()
        {
            Map = new ClientChunkMap(this);
        }
    }
}
