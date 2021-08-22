using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Ticker
    {
        private TimeSpan _tick_delay = TimeSpan.Zero;
        private bool _running = false;
        private DateTime _lastTick = DateTime.MinValue;

        private DateTime _nextTick { get => _lastTick + _tick_delay; }

        public double TPS => _delays.Sum() / 10;

        public Ticker(int ticksPerSecond = 20)
        {
            _tick_delay = TimeSpan.FromMilliseconds(1000 / ticksPerSecond);
        }

        private List<double> _delays = new List<double>(10);

        public void Run(Action tick)
        {
            _running = true;
            while (_running)
            {
                var now = DateTime.UtcNow;
                if (now > _nextTick)
                {
                    _lastTick = DateTime.UtcNow;
                    tick();
                    if (_delays.Count == 10)
                        _delays.RemoveAt(0);
                    _delays.Add((DateTime.UtcNow - now).TotalMilliseconds);
                }
            }
        }
    }
}
