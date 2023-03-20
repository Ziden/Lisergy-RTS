using Game.Events;

namespace Game.Entity
{
    /// <summary>
    /// Can send status updates after status changes (e.g after battles)
    /// </summary>
    public interface IStatusUpdateable
    {
        ServerPacket GetStatusUpdatePacket();
    }
}
