using Assets.Code.World;
using Game;
using GameData;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class ClientStrategyGame : StrategyGame
    {
        public ClientStrategyGame(GameConfiguration cfg, GameSpec specs, GameWorld world) : base(cfg, specs, world) {
           
        }

        public ClientWorld GetWorld()
        {
            return World as ClientWorld;
        }
    }
}
