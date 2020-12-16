using Assets.Code.World;
using Game;
using Game.Events;
using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    class GameListener
    {
        public static StrategyGame Game;

        public GameListener()
        {
            EventSink.OnSpecResponse += ReceiveSpecs;
        }

        public void ReceiveSpecs(GameSpecResponse ev)
        {
            var world = new ClientWorld();
            world.CreateWorld(ev.Cfg.WorldMaxPlayers);
            Game = new StrategyGame(ev.Cfg, ev.Spec, world);
            Log.Debug("Game Specs & Config Received for "+ ev.Cfg.WorldMaxPlayers+" Max Players");
        }
    }
}
