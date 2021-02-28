
using Assets.Code.World;
using Game;
using Game.Entity;

namespace Assets.Code
{
    public class ClientPlayer : PlayerEntity
    {

        public ClientPlayer() : base()
        {
            StackLog.Debug("Created new player");
        }

        public override bool Online()
        {
            return true;
        }

        public override void Send<T>(T ev)
        {
            MainBehaviour.Networking.Send((T)ev);
        }
    }
}
