using Game;
using Game.DataTypes;
using Game.Systems.Player;
using Game.Tile;
using Game.World;

namespace Assets.Code.World
{
    public class ClientWorld : GameWorld
    {
        public ClientWorld(in ushort sizeX, in ushort sizeY): base(int.MaxValue, sizeX, sizeY)
        {
        }

        public TileEntity GetTile(BaseEntity e)
        {
            return Map.GetTile(e.Tile.X, e.Tile.Y);
        }

        public PlayerEntity GetOrCreateClientPlayer(GameId uid)
        {
            PlayerEntity pl;
            if (Players.GetPlayer(uid, out pl))
                return pl;
            if (uid == MainBehaviour.LocalPlayer.EntityId)
            {
                Players.Add(MainBehaviour.LocalPlayer);
                return MainBehaviour.LocalPlayer;
            }
            pl = new OtherPlayer(uid);
            Players.Add(pl);
            return pl;
        }

        public void CreateMap()
        {
            Map = new ClientChunkMap(this);
        }
    }
}
