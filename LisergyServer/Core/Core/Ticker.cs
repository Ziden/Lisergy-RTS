using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseServer.Core
{
    public class Ticker
    {
        private readonly TimeSpan _tick_delay = TimeSpan.Zero;
        private bool _running = false;
        private DateTime _lastTick = DateTime.MinValue;

        private DateTime _nextTick => _lastTick + _tick_delay;

        public double TPS => _delays.Sum() / 10;

        public Ticker(int ticksPerSecond = 20)
        {
            _tick_delay = TimeSpan.FromMilliseconds(1000 / ticksPerSecond);
        }

        private readonly List<double> _delays = new(10);

        public void Run(Action tick)
        {
            _running = true;
            while (_running)
            {
                DateTime now = DateTime.UtcNow;
                if (now > _nextTick)
                {
                    _lastTick = DateTime.UtcNow;
                    tick();
                    if (_delays.Count == 10)
                    {
                        _delays.RemoveAt(0);
                    }

                    _delays.Add((DateTime.UtcNow - now).TotalMilliseconds);
                }
            }
        }
    }
}
