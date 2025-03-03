namespace Game.Engine.Network
{
    public interface IGameCommand
    {
        void Execute(IGame game);
    }

    public interface IClientPacket
    {

    }
}
