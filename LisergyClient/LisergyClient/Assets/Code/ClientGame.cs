using Assets.Code.World;
using Game;
using Game.Events.Bus;
using GameData;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class ClientStrategyGame : StrategyGame
    {
        public ClientStrategyGame(GameSpec specs, GameWorld world) : base(specs, world)
        {
        }

        public ClientWorld GetWorld()
        {
            return World as ClientWorld;
        }

        public override void RegisterEventListeners()
        {
         
        }
    }
}
