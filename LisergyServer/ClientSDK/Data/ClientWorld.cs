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
        public ClientWorld(int maxPlayers, in ushort sizeX, in ushort sizeY) : base(maxPlayers, sizeX, sizeY)
        {
        }

        public override void CreateMap()
        {
           Map = new LazyLoadChunkMap(this, SizeX, SizeY);
        }
    }
}
