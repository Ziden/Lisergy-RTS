using System;
using System.Collections.Generic;

namespace Assets.Code
{
    public delegate bool WaitCondition();
    public delegate void WaitAction();

    public class Awaiter
    {
        private static Dictionary<string, DateTime> _cds = new Dictionary<string, DateTime>();

        public static void SetCooldown(string name, TimeSpan time)
        {
            _cds[name] = DateTime.UtcNow + time;
        }

        public static int CooldownSecondsRemaining(string name)
        {
            if (!_cds.ContainsKey(name))
                return 0;
            return (_cds[name] - DateTime.UtcNow).Seconds;
        }

        public static bool IsCooldown(string name)
        {
            if (!_cds.ContainsKey(name))
                return false;
            return _cds[name] > DateTime.UtcNow;
        }

        private static List<Tuple<WaitCondition, WaitAction>> _waiting = new List<Tuple<WaitCondition, WaitAction>>();
        private static List<Tuple<WaitCondition, WaitAction>> _done = new List<Tuple<WaitCondition, WaitAction>>();

        public static void WaitFor(TimeSpan time, WaitAction action)
        {
            var endTime = DateTime.Now + time;
            Wait(() => DateTime.Now > endTime, action);
        }

        public static void Wait(WaitCondition condition, WaitAction action)
        {
            if (condition())
                action();
            else
                _waiting.Add(new Tuple<WaitCondition, WaitAction>(condition, action));
        }

        public static void Update()
        {
            foreach (var tuple in _waiting)
            {
                if(tuple.Item1())
                {
                    _done.Add(tuple);
                }
            }
            _done.ForEach(d => {
                _waiting.Remove(d);
                d.Item2();
            });
            _done.Clear();
        }

    }
}
