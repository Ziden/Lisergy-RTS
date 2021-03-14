using Game;
using Game.Entity;
using Game.Events.ServerEvents;
using Game.World;
using System.Collections.Generic;

namespace Assets.Code.World
{
    public class ClientWorld : GameWorld
    {
        public ClientWorld(GameSpecResponse gameSpecs): base(int.MaxValue, gameSpecs.WorldX, gameSpecs.WorldY)
        {
        }

        public Dictionary<string, Party> Parties = new Dictionary<string, Party>();

        public ClientPlayer GetOrCreateClientPlayer(string uid)
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

        // In client we generate the tiles on-demand
        public override void CreateChunkMap()
        {
            ChunkMap = new ClientChunkMap(this);
        }
    }
}
