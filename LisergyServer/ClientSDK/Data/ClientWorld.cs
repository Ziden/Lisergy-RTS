using Game;
using Game.DataTypes;
using Game.Systems.Player;
using Game.World;


namespace ClientSDK.Data
{

    /// <summary>
    /// Implementation of the game world for the client SDK.
    /// Instead of requiring to have all tiles generated, it will lazy generate tiles.
    /// </summary>
    public interface IClientWorld { }

    public class ClientWorld : GameWorld, IClientWorld
    {
        public ClientWorld(IGame game, in ushort sizeX, in ushort sizeY) : base(game, sizeX, sizeY)
        {
            Players = new LazyLoadedPlayers(game);
        }

        public override void CreateMap()
        {
           Map = new LazyLoadChunkMap(this, SizeX, SizeY);
        }
    }
}
