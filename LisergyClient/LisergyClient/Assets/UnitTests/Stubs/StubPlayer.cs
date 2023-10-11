
using Game;
using Game.Systems.Player;
using System.Collections.Concurrent;

namespace Assets.UnitTests.Stubs
{
    public class StubPacket
    {
        public byte[] Data;
        public PlayerEntity Sender;
    }

    public class StubPlayer : PlayerEntity
    {
        public StubPlayer() : base(null)
        {
        }

      
    }
}
