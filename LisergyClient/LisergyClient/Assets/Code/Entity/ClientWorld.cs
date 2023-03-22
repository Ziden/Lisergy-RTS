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

        public PlayerEntity GetOrCreateClientPlayer(GameId uid)
        {
            PlayerEntity pl;
            if (Players.GetPlayer(uid, out pl))
                return pl;
            if (uid == MainBehaviour.LocalPlayer.UserID)
            {
                Players.Add(MainBehaviour.LocalPlayer);
                return MainBehaviour.LocalPlayer;
            }
            pl = new OtherPlayer(uid);
            Players.Add(pl);
            return pl;
        }

        public override void CreateMap()
        {
            Map = new ClientChunkMap(this);
        }
    }
}
