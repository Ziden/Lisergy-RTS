using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public delegate bool WaitCondition();
    public delegate void WaitAction();

    public class Awaiter
    {
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
                    tuple.Item2();
                }
            }
            _done.ForEach(d => _waiting.Remove(d));
            _done.Clear();
        }

    }
}
