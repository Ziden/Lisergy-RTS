using Game;
using Game.Entity;
using Game.Events.ServerEvents;
using Game.World;
using System;
using System.Collections.Generic;

namespace Assets.Code.World
{
    public class ClientWorld : GameWorld
    {
        public ClientWorld(int sizeX, int sizeY): base(int.MaxValue, sizeX, sizeY)
        {
        }

        public ClientTile GetClientTile(int tileX, int tileY)
        {
            return Map.GetTile(tileX, tileY) as ClientTile;
        }

        public ClientTile GetClientTile(WorldEntity e)
        {
            return Map.GetTile(e.X, e.Y) as ClientTile;
        }

        public ClientPlayer GetOrCreateClientPlayer(GameId uid)
        {
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

        public override void CreateMap()
        {
            Map = new ClientChunkMap(this);
        }
    }
}
