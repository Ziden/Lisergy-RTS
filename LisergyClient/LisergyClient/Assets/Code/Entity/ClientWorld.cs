using Game;
using Game.DataTypes;
using Game.Player;
using Game.Tile;

namespace Assets.Code.World
{
    public class ClientWorld : GameWorld
    {
        public ClientWorld(int sizeX, int sizeY): base(int.MaxValue, sizeX, sizeY)
        {
        }

        public TileEntity GetTile(WorldEntity e)
        {
            return Map.GetTile(e.X, e.Y);
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
