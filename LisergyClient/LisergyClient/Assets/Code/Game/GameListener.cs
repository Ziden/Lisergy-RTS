using Game;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public class GameListener : IEventListener
    {
        [EventMethod]
        public void OnMessage(MessagePopupPacket p)
        {
            Log.Debug("--- Message from server---");
            foreach (var a in p.Args)
                Log.Debug(a);
        }
    }
}
