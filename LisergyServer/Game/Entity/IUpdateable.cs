using Game.Events;

namespace Game.Entity
{
    public interface IStatusUpdateable
    {
        ServerPacket GetStatusUpdatePacket();
    }
}
