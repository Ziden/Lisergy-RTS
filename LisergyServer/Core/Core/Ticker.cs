using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BaseServer.Core
{
    public class Ticker
    {
        private readonly TimeSpan _tick_delay = TimeSpan.Zero;
        private bool _running = false;
        private DateTime _lastTick = DateTime.MinValue;

        public DateTime NextTick => _lastTick + _tick_delay;

        public double TPS => _delays.Sum() / 10;

        public Ticker(int ticksPerSecond = 20)
        {
            _tick_delay = TimeSpan.FromMilliseconds(1000 / ticksPerSecond);
        }

        public void Stop() => _running = false;

        private readonly List<double> _delays = new(10);

        public void Run(Action tick)
        {
            _running = true;
            while (_running)
            {
                if (DateTime.UtcNow > NextTick)
                {
                    _lastTick = DateTime.UtcNow;
                    tick();
                    var timeUntilNextTick = (NextTick - DateTime.UtcNow).TotalMilliseconds;
                    if (timeUntilNextTick > 0)
                    {
                        Thread.Sleep((int)timeUntilNextTick); // finished early, sleep thread to allow others to run
                    }
                }
            }
        }
    }
}
