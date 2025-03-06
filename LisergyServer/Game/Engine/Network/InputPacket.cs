namespace Game.Engine.Network
{
    public interface IGameCommand : IPacket
    {
        void Execute(IGame game);
    }

    public interface IClientPacket : IPacket
    {

    }
}
