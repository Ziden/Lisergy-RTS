
using Game;
using Game.Events;

namespace Assets.Code
{
    public class ClientPlayer : PlayerEntity
    {
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
