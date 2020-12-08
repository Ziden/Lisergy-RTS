using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LegendsServer.Core
{
    public class ServerTicks
    {
        public static ManualResetEvent TickSignal = new ManualResetEvent(false);

        private static int TICKS_PER_SECOND = 2;

        public static bool Ticking = true;

        private DateTime lastTick;

        public void StartTicks()
        {
            var tickSleepMS = 1000d / TICKS_PER_SECOND;
            while (Ticking)
            {
                var now = DateTime.UtcNow;
                //Log.Debug($"Tick [{(now-lastTick).TotalMilliseconds}ms]");

                GameTick(now);
                this.lastTick = now;
                var elapsed = DateTime.UtcNow - now;

                var delay = tickSleepMS - elapsed.TotalMilliseconds;
                if(delay > 0)
                    Thread.Sleep((int)delay);
            }
        }

        public void GameTick(DateTime date)
        {

        }
    }
}
