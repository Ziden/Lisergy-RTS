using Game.Engine;
using Game.Engine.Events;
using NUnit.Framework;
using System;

namespace GameUnitTests
{
    [TestFixture]
    public class TestFixture : IDisposable
    {
        public TestFixture()
        {
            // runs before all tests
        }

        [TearDown]
        public void Dispose()
        {
            UnmanagedMemory.FlagMemoryToBeReused();
            if (EventPoolValidator.Used.Count > 0)
            {
                throw new Exception("Some events have not been returned to pool: " + string.Join(',', EventPoolValidator.Used));
            }
            Console.WriteLine("Disposing Test");
        }
    }
}
