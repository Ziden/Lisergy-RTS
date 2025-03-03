using Game;
using Game.Engine.DataTypes;
using Game.Entities;
using Game.Systems.Player;
using Game.World;

namespace ClientSDK.Data
{

    /// <summary>
    /// Implementation of the game world for the client SDK.
    /// Instead of requiring to have all tiles generated, it will lazy generate tiles.
    /// </summary>
    public interface IClientWorld { }

    /// <summary>
    /// Will create players as players are requests but not available
    /// </summary>
    public class LazyLoadedPlayers : WorldPlayers
    {
        private IGame _game;

        public LazyLoadedPlayers(IGame game) : base(int.MaxValue)
        {
            _game = game;
        }

        public override PlayerModel GetPlayer(GameId id)
        {
            var p = base.GetPlayer(id);
            if (p == null) _players[id] = new OtherPlayer(_game, _game.Entities.CreateEntity(EntityType.Player, GameId.ZERO, id));
            return base.GetPlayer(id);
        }
    }

    public class ClientWorld : GameWorld, IClientWorld
    {
        public ClientWorld(IGame game, in ushort sizeX, in ushort sizeY) : base(game, sizeX, sizeY)
        {
            Players = new LazyLoadedPlayers(game);
        }

        public override void CreateMap()
        {
            this.Chunks = new ServerChunkMap(this, SizeX, SizeY); // TODO: Remove
        }
    }
}
